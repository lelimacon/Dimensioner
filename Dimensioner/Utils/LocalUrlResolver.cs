using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Dimensioner.Utils
{
    internal class LocalUrlResolver : IDisposable
    {
        private static readonly XmlUrlResolver XmlUrlResolver;
        private static string _entryPoint;

        // Zip reading
        private readonly static Object ArchiveLock;
        private readonly string _tmpDir;
        private FileStream _memoryStream;
        private ZipArchive _archive;
        private string _archivePath;

        public bool UseCache { get; }
        public string CacheDir { get; }
        public IWebProxy Proxy { get; }
        public static string RootDir { get; private set; }
        public static string EntryPoint
        {
            get { return _entryPoint; }
            set
            {
                // Clean path.
                _entryPoint = Resolve(null, value);

                // Taxonomy probably changed: reset root directory.
                RootDir = null;
            }
        }

        static LocalUrlResolver()
        {
            XmlUrlResolver = new XmlUrlResolver();
            ArchiveLock = new Object();
        }

        public LocalUrlResolver(ReaderConfiguration configuration, string tmpDir)
        {
            UseCache = configuration.UseCache;
            CacheDir = configuration.CacheDir;
            Proxy = configuration.Proxy;
            _tmpDir = tmpDir;
        }

        public void Dispose()
        {
            _memoryStream?.Dispose();
            _archive?.Dispose();
            if (_archivePath != null)
                File.Delete(_archivePath);
        }

        /// <summary>
        ///     Loads an archive and returns all of its entries.
        ///     If the archive is online, download it to a temporary folder.
        /// </summary>
        /// <param name="path">The archive path, can be online.</param>
        /// <returns>The (new) path to the archive, and the entries relative path to the archive.</returns>
        public (string, IEnumerable<string>) LoadArchive(string path)
        {
            if (_archive != null)
                throw new Exception($"An archive is already loaded");

            var uri = new Uri(path);

            // If file is on the web, save archive in temporary folder.
            if (!uri.IsFile)
            {
                string name = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
                path = $"{_tmpDir}/{name}-{StringUtils.RandomString(8)}.zip";
                _archivePath = path;
                DownloadFile(uri, path);
            }

            // Initialize the archive.
            _memoryStream = new FileStream(path, FileMode.Open);
            _archive = new ZipArchive(_memoryStream, ZipArchiveMode.Read);
            return (path, _archive.Entries.Select(e => e.FullName));
        }

        public StreamReader GetEntity(string path)
        {
            if (path.Contains(".zip"))
                return GetZipEntry(path);
            return GetEntity(new Uri(path));
        }

        private StreamReader GetZipEntry(string path)
        {
            if (_archive == null)
                throw new Exception($"Attempt to read an unloading archive at {path}");
            const string ext = ".zip";
            string entryName = path.Substring(path.IndexOf(ext) + ext.Length + 1);
            ZipArchiveEntry entry = _archive.GetEntry(entryName);
            string fileContent;
            lock (_archive)
            {
                using (Stream stream = entry.Open())
                using (var reader = new StreamReader(stream))
                    fileContent = reader.ReadToEnd();
            }
            byte[] buffer = Encoding.UTF8.GetBytes(fileContent);
            return new StreamReader(new MemoryStream(buffer));
        }

        public StreamReader GetEntity(Uri absoluteUri)
        {
            // Is file local.
            if (absoluteUri.IsFile)
                return new StreamReader(absoluteUri.LocalPath);

            // File is on the web.
            var relativePath = absoluteUri.AbsoluteUri.Substring(7);

            // Check the taxonomy root directory.
            if (EntryPoint != null && RootDir != null || FindRootDir(EntryPoint, relativePath) != null)
            {
                string rootPath = Path.Combine(RootDir, relativePath);
                if (File.Exists(rootPath))
                {
                    byte[] buffer = File.ReadAllBytes(rootPath);
                    var stream = new MemoryStream(buffer);
                    return new StreamReader(stream);
                }
            }

            // Send web stream if cache denied.
            if (!UseCache)
            {
                var client = new WebClient {Proxy = Proxy};
                var stream = client.OpenRead(absoluteUri);
                return new StreamReader(stream);
            }

            // Check the cache.
            string cachePath = Path.Combine(CacheDir, relativePath);

            // If the file is not cached, download it and cache it.
            if (!File.Exists(cachePath))
                DownloadFile(absoluteUri, cachePath);

            return new StreamReader(cachePath);
        }

        private string FindRootDir(string sourceDir, string relativePath)
        {
            if (File.Exists(Path.Combine(sourceDir, relativePath)))
            {
                RootDir = sourceDir;
                return sourceDir;
            }
            string parentDir = Directory.GetParent(sourceDir)?.FullName;
            return string.IsNullOrEmpty(parentDir) ? null : FindRootDir(parentDir, relativePath);
        }

        /// <summary>
        ///     Tries to download the file, and deletes it if there is a problem.
        ///     Ensures that the folder to the path is created.
        /// </summary>
        /// <param name="source">The path to the web resource.</param>
        /// <param name="targetPath">The path to download the file to.</param>
        private void DownloadFile(Uri source, string targetPath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                using (var client = new WebClient { Proxy = Proxy })
                {
                    client.DownloadFile(source.AbsoluteUri, targetPath);
                }

                // Check file size (404 will produce empty file).
                var file = new FileInfo(targetPath);
                if (file.Length == 0)
                    throw new Exception($"File downloaded from '{source}' is empty (probably 404)");
            }
            catch
            {
                File.Delete(targetPath);
                throw;
            }
        }

        public static string Resolve(string sourceUri, string path)
        {
            // Null check.
            if (string.IsNullOrEmpty(sourceUri) || IsAbsoluteUri(path))
                return path.Replace('\\', '/');

            // Exception for zip files.
            if (sourceUri.EndsWith(".zip"))
                return Path.Combine(sourceUri, path).Replace('\\', '/');

            // Classic resolve.
            Uri uri = ResolveUri(new Uri(sourceUri), path);
            if (uri.IsFile)
                return Uri.UnescapeDataString(uri.AbsolutePath).Replace('\\', '/');
            return Uri.UnescapeDataString(uri.AbsoluteUri);
        }

        public static bool IsAbsoluteUri(string uri)
        {
            //if (!Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
            //    throw new ArgumentException("URL was in an invalid format", nameof(uri));

            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }

        public static Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            return XmlUrlResolver.ResolveUri(baseUri, relativeUri);
        }
    }
}
