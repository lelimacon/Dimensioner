using Dimensioner.Utils;
using System.Net;

namespace Dimensioner
{
    public class ReaderConfiguration
    {
        /// <summary>
        ///     Use the cache directory (true by default).
        /// </summary>
        public bool UseCache { get; set; }

        /// <summary>
        ///     Specify the cache directory (%appdata%/{AppName} by default).
        /// </summary>
        public string CacheDir { get; set; }

        /// <summary>
        ///     Specify a proxy.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        public ReaderConfiguration()
        {
            var cacheManager = new CacheManager(true);
            CacheDir = cacheManager.SubDir("Cache");
            UseCache = true;
            Proxy = null;
        }
    }
}
