using System.Net;
using Dimensioner.Utils;

namespace Dimensioner
{
    public class ReaderConfiguration
    {
        internal LocalUrlResolver UrlResolver { get; }

        /// <summary>
        ///     Use the cache directory (true by default).
        /// </summary>
        public bool UseCache
        {
            get => UrlResolver.UseCache;
            set => UrlResolver.UseCache = value;
        }

        /// <summary>
        ///     Specify the cache directory (%appdata%/{AppName} by default).
        /// </summary>
        public string CacheDir
        {
            get => UrlResolver.CacheDir;
            set => UrlResolver.CacheDir = value;
        }

        public IWebProxy Proxy
        {
            get => UrlResolver.Proxy;
            set => UrlResolver.Proxy = value;
        }

        public ReaderConfiguration()
        {
            UrlResolver = new LocalUrlResolver();
            UseCache = true;
        }
    }
}
