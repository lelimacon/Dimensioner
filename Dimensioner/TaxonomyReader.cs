using System;
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
        private class ReadingInstance
        {
            public bool Done { get; set; }
            public XbrlSchema Schema { get; set; }

            public ReadingInstance(XbrlSchema schema)
            {
                Schema = schema;
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
        public IReadOnlyList<ComponentReaderTracker> ComponentReaders => _componentReaders;
        public IReadOnlyList<ComponentReaderException> Errors => _errors.ToList();

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
            path = Path.GetFullPath(path);
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
                Thread.Sleep(100);
            } while (_instances.Any(i => !i.Value.Done));

            Reading = false;

            // Create schema set and add instances.
            var schemaSet = new XbrlSchemaSet();
            schemaSet.Add(_instances.Select(i => i.Value.Schema));

            // Let the components post process the schema set.
            ReadComponents(schemaSet, null);

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
            ThreadPool.QueueUserWorkItem(i => ReadSafe(i), instance, true);
        }

        private XbrlSchema ReadSafe(ReadingInstance instance)
        {
            XbrlSchema schema = null;
            try
            {
                schema = Read(instance);
            }
            catch (Exception e)
            {
                _errors.Add(new ComponentReaderException(typeof(TaxonomyReader), e));
            }
            return schema;
        }

        private XbrlSchema Read(ReadingInstance instance)
        {
            //Console.WriteLine($"Reading instance {instance.Schema.Path}");
            XbrlSchema schema = instance.Schema;
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

            instance.Done = true;
            return schema;
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

        public static IEnumerable<LocatorNode> ToLocatorGraph(
            string basePath, XElement node, bool tree)
        {
            var locs = node.Children(Ns.Link, "loc")
                .Select(n => new LocatorNode(basePath, n))
                .ToList();
            var arcs = node.Elements()
                .Where(n => n.Name.Namespace == Ns.Link
                            && n.Attr(Ns.Xlink, "type") == "arc")
                .Select(n => new Arc(basePath, n))
                .OrderBy(a => a.Order)
                .ToList();

            // Index the access.
            var fromArcs = arcs.ToLookup(a => a.From);
            var toArcs = arcs.ToLookup(a => a.To);

            // Fetch child and parent locators.
            foreach (LocatorNode loc in locs)
            {
                var childArcs = fromArcs[loc.Locator.Label].ToList();
                var childLocs = locs.Where(l => childArcs.Any(a => a.To == l.Locator.Label));
                loc.Children = childLocs.ToList();

                var parentArcs = toArcs[loc.Locator.Label].ToList();
                var parentLocs = locs.Where(l => parentArcs.Any(a => a.From == l.Locator.Label));
                loc.Parents = parentLocs.ToList();

                if (tree && parentArcs.Count > 1)
                    throw new Exception("Multiple parents were not expected.");

                loc.ChildArcs = childArcs;
            }

            // Return roots (locators with no parents).
            return locs.Where(l => l != null && !l.Parents.Any());
        }
    }
}
