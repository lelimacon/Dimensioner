using System;
using System.IO;

namespace Dimensioner.Utils
{
    public class CacheManager
    {
        private static string _appDataPath;
        private static string _cachePath;

        public string CompanyName { get; }

        public string AppName { get; }

        /// <summary>
        ///     Gets the path of the Windows AppData directory.
        /// </summary>
        public static string AppDataDir => _appDataPath ??
            (_appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        /// <summary>
        ///     Gets the path to this application's appdata folder.
        /// </summary>
        public string CacheDir => _cachePath ??
            (_cachePath = Path.Combine(AppDataDir, CompanyName, AppName));

        /// <summary>
        ///     Infers the company name and application name from the executing assembly.
        /// </summary>
        public CacheManager(bool ignoreCompany)
        {
            CompanyName = ignoreCompany ? "" : AssemblyUtils.CompanyName;
            AppName = AssemblyUtils.ProductName;
        }

        public CacheManager(string companyName, string appName)
        {
            CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
            AppName = appName ?? throw new ArgumentNullException(nameof(appName));
        }

        public string SubDir(string name)
        {
            return Path.Combine(CacheDir, name);
        }

        /// <summary>
        ///     Save a file to the specified location.
        /// </summary>
        public void Save(string filename, byte[] content, bool overwrite = true)
        {
            string path = Path.Combine(CacheDir, filename);
            if (!overwrite && File.Exists(path))
                return;
            File.WriteAllBytes(path, content);
        }

        /// <summary>
        ///     Save a file to the specified location.
        /// </summary>
        public void Save(string subFolder, string filename, byte[] content, bool overwrite = true)
        {
            string path = Path.Combine(SubDir(subFolder), filename);
            if (!overwrite && File.Exists(path))
                return;
            File.WriteAllBytes(path, content);
        }
    }
}
