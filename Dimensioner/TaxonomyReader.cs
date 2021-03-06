﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Dimensioner.Components;
using Dimensioner.Utils;

namespace Dimensioner
{
    public class TaxonomyReader : IDisposable
    {
        public enum ReadingStatus
        {
            Pending,
            Reading,
            Success,
            Error,
        }

        public class ReadingInstance
        {
            public ReadingStatus Status { get; internal set; }
            public XbrlSchema Schema { get; internal set; }

            public bool Done => Status == ReadingStatus.Success || Status == ReadingStatus.Error;

            public ReadingInstance(XbrlSchema schema)
            {
                Schema = schema;
                Status = ReadingStatus.Pending;
            }
        }

        public class ComponentReaderTracker
        {
            public TimeSpan Elapsed { get; set; }
            public TaxonomyComponentReader Reader { get; set; }

            public ComponentReaderTracker(TaxonomyComponentReader reader)
            {
                Reader = reader;
            }
        }


        private readonly ConcurrentDictionary<string, ReadingInstance> _instances;
        private readonly List<ComponentReaderTracker> _componentReaders;
        private readonly ConcurrentBag<ComponentReaderException> _errors;
        private readonly LocalUrlResolver _urlResolver;

        public bool Reading { get; private set; }
        public IReadOnlyList<ReadingInstance> ReadingInstances => _instances.Select(i => i.Value).ToList();
        public IReadOnlyList<ComponentReaderTracker> ComponentReaders => _componentReaders;
        public IReadOnlyList<ComponentReaderException> Errors => _errors.ToList();

        public event Action<ReadingInstance> ReadingInstanceStarted;
        public event Action<ReadingInstance> ReadingInstanceEnded;

        public TaxonomyReader(ReaderConfiguration configuration)
        {
            _instances = new ConcurrentDictionary<string, ReadingInstance>();
            _componentReaders = new List<ComponentReaderTracker>();
            _errors = new ConcurrentBag<ComponentReaderException>();
            var cacheManager = new CacheManager(true);
            _urlResolver = new LocalUrlResolver(configuration, cacheManager.SubDir("Temporary"));

            Reading = false;
        }

        public void Dispose()
        {
            _urlResolver.Dispose();
        }

        public TaxonomyReader Register<T>()
            where T : TaxonomyComponentReader, new()
        {
            TaxonomyComponentReader reader = new T();
            return Register(reader);
        }

        public TaxonomyReader Register<T>(T componentReader)
            where T : TaxonomyComponentReader
        {
            componentReader.BaseReader = this;
            _componentReaders.Add(new ComponentReaderTracker(componentReader));
            return this;
        }

        public XbrlSchemaSet Read(string path)
        {
            // Format the path.
            path = LocalUrlResolver.Resolve(Path.GetFullPath("."), path);
            LocalUrlResolver.EntryPoint = path;

            // Load an archive.
            if (path.Contains(".zip"))
                return ReadZip(path);

            if (Reading)
                throw new Exception("Another entry point is already being read with this instance.");
            Reading = true;

            // Load a directory.
            if (Directory.Exists(path))
            {
                var directory = new DirectoryInfo(path);
                QueueChildren(directory);
            }

            // Load a schema (entry point).
            else
            {
                var baseSchema = new XbrlSchema(path);
                Queue(baseSchema);
            }

            return Read();
        }

        public XbrlSchemaSet ReadZip(string path)
        {
            // Format the path.
            path = Path.GetFullPath(path);
            LocalUrlResolver.EntryPoint = path;

            if (Reading)
                throw new Exception("Another entry point is already being read with this instance.");
            Reading = true;

            (var tmpPath, var entries) = _urlResolver.LoadArchive(path);
            foreach (var entry in entries.Where(e => e.EndsWith(".xsd")))
                Queue(tmpPath, entry, null);
            return Read();
        }

        private void QueueChildren(DirectoryInfo directory)
        {
            var schemas = directory.GetFiles().Where(f => f.Name.EndsWith(".xsd"));
            foreach (var schema in schemas)
                Queue(null, schema.FullName, null);
            foreach (var subDir in directory.GetDirectories())
                QueueChildren(subDir);
        }
        
        private XbrlSchemaSet Read()
        {
            // Wait for all threads in pool to calculate.
            do
            {
                Thread.Sleep(300);
            } while (_instances.Any(i => !i.Value.Done));

            Reading = false;

            Console.WriteLine("Finished reading.");
            Console.WriteLine("Post processing...");

            // Create schema set and add instances.
            var schemaSet = new XbrlSchemaSet();
            schemaSet.Add(_instances.Select(i => i.Value.Schema));

            // Let the components post process the schema set.
            ReadComponents(schemaSet, null);

            Console.WriteLine("Done.");
            return schemaSet;
        }

        /// <summary>
        ///     Adds an XBRL schema to the thread pool for reading.
        ///     Is meant to be accessed internally and by taxonomy component readers.
        /// </summary>
        /// <param name="basePath">The origin path, can be null.</param>
        /// <param name="path">The relative or absolute path of the schema.</param>
        /// <param name="ns">The expected target namespace, can be null.</param>
        /// <returns>The empty shell of the future schema.</returns>
        internal XbrlSchema Queue(string basePath, string path, string ns)
        {
            if (!Reading)
                throw new Exception("Instances must be pushed while reading the taxonomy.");

            path = LocalUrlResolver.Resolve(basePath, path);
            XbrlSchema schema;
            lock (_instances)
            {
                if (_instances.ContainsKey(path))
                {
                    schema = _instances[path].Schema;
                }
                else
                {
                    schema = new XbrlSchema(path, ns);
                    Queue(schema);
                }
            }
            return schema;
        }

        private void Queue(XbrlSchema schema)
        {
            var instance = new ReadingInstance(schema);
            _instances[schema.Path] = instance;
            ThreadPool.QueueUserWorkItem(i => ReadSafe((ReadingInstance)i), instance);
        }

        private XbrlSchema ReadSafe(ReadingInstance instance)
        {
            ReadingInstanceStarted?.Invoke(instance);
            try
            {
                Read(instance.Schema);
                instance.Status = ReadingStatus.Success;
            }
            catch (Exception e)
            {
                instance.Status = ReadingStatus.Error;
                _errors.Add(new ComponentReaderException(typeof(TaxonomyReader), e));
            }
            ReadingInstanceEnded?.Invoke(instance);
            return instance.Schema;
        }

        private void Read(XbrlSchema schema)
        {
            //Console.WriteLine($"Reading instance {instance.Schema.Path}");
            XDocument doc;
            using (var stream = GetEntity(schema.Path))
            {
                doc = XDocument.Load(stream, LoadOptions.None);
            }
            XElement root = doc.Root;
            if (root.Name.Namespace != Ns.Xs)
                throw new ArgumentException("Specified file is not an XML schema");

            // Check target namespace.
            string ns = root.Attr("targetNamespace");
            if (schema.Namespace != null && schema.Namespace != ns)
                throw new Exception("Target namespace is not equal to provided namespace");
            schema.Namespace = ns;

            // Retrieve and enqueue children (imports).
            var children = root.Children(Ns.Xs, "import")
                .Select(e => ConvertImport(schema.Path, e))
                .ToList();
            schema.Children = children;

            // Read linkbases.
            var linkbases = root.Children(Ns.Xs, "annotation")
                .SelectMany(n => n.Children(Ns.Xs, "appinfo"))
                .SelectMany(n => n.Children(Ns.Link, "linkbaseRef"))
                .Select(n => ReadLinkbase(schema, n))
                .ToList();

            // Wind up all linkbase components to the underlying schema.
            schema.Add(linkbases.SelectMany(l => l.Comps).Distinct());

            // Let the components read the schema.
            ReadComponents(schema, doc);
        }

        private StreamReader GetEntity(string path)
        {
            return _urlResolver.GetEntity(path);
        }

        private Linkbase ReadLinkbase(XbrlSchema schema, XElement node)
        {
            var xlink = new XlinkNode(schema.Path, node);
            string path = xlink.Href.AbsolutePath;
            var linkbase = new Linkbase(schema, path, xlink.Role);
            XDocument doc;
            using (var stream = GetEntity(path))
            {
                doc = XDocument.Load(stream, LoadOptions.None);
            }
            if (doc.Root.Name != (XNamespace) Ns.Link + "linkbase")
                throw new ArgumentException("Specified XML file is not a linkbase");

            // Let the components read the linkbase.
            ReadComponents(linkbase, doc);

            return linkbase;
        }

        private void ReadComponents(Modulable modulable, XDocument document)
        {
            foreach (var tracker in _componentReaders)
            {
                var timer = new Stopwatch();
                IEnumerable<TaxonomyComponent> components = null;
                timer.Start();
                try
                {
                    components = tracker.Reader.Read(modulable, document);
                }
                catch (Exception e)
                {
                    _errors.Add(new ComponentReaderException(tracker.Reader.GetType(), e));
                }
                timer.Stop();
                tracker.Elapsed += timer.Elapsed;
                modulable.Add(components);
            }
        }

        private XbrlSchema ConvertImport(string basePath, XElement node)
        {
            string ns = node.Attr("namespace");
            string relPath = node.Attr("schemaLocation");
            return Queue(basePath, relPath, ns);
        }
    }
}
