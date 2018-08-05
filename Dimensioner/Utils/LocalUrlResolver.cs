using System;
using System.IO;
using System.Net;
using System.Xml;

namespace Dimensioner.Utils
{
    public class LocalUrlResolver
    {
        private static readonly XmlUrlResolver XmlUrlResolver;

        public bool UseCache { get; set; }
        public string CacheDir { get; set; }
        public IWebProxy Proxy { get; set; }

        static LocalUrlResolver()
        {
            XmlUrlResolver = new XmlUrlResolver();
        }

        public LocalUrlResolver()
        {
            var cacheManager = new CacheManager();
            CacheDir = cacheManager.SubDir("Cache");
        }

        public StreamReader GetEntity(string path)
        {
            return GetEntity(new Uri(path));
        }

        public StreamReader GetEntity(Uri absoluteUri)
        {
            if (absoluteUri.IsFile)
                return new StreamReader(absoluteUri.LocalPath);

            if (!UseCache)
            {
                var client = new WebClient {Proxy = Proxy};
                var stream = client.OpenRead(absoluteUri);
                return new StreamReader(stream);
            }

            // File is on the web, so first check the cache.
            var relativePath = absoluteUri.AbsoluteUri.Substring(7);
            string cachePath = Path.Combine(CacheDir, relativePath);

            // If the file is not cached, download it and cache it.
            if (!File.Exists(cachePath))
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(cachePath));
                    var client = new WebClient {Proxy = Proxy};
                    client.DownloadFile(absoluteUri.AbsoluteUri, cachePath);
                }
                catch
                {
                    File.Delete(cachePath);
                    throw;
                }

            return new StreamReader(cachePath);
        }

        public static string Resolve(string sourceUri, string path)
        {
            if (string.IsNullOrEmpty(sourceUri) || IsAbsoluteUri(path))
                return path.Replace('\\', '/');
            Uri uri = ResolveUri(new Uri(sourceUri), path);
            if (uri.IsFile)
                return Uri.UnescapeDataString(uri.AbsolutePath).Replace('\\', '/');
            return Uri.UnescapeDataString(uri.AbsoluteUri);
        }

        public static bool IsAbsoluteUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
                throw new ArgumentException("URL was in an invalid format", nameof(uri));

            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }

        public static Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            return XmlUrlResolver.ResolveUri(baseUri, relativeUri);
        }
    }
}
