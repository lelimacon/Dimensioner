using System;
using System.IO;

namespace Dimensioner.Utils
{
    public class CacheManager
    {
        private static string _generalAppDataPath;
        private static string _appDataPath;

        public string CompanyName { get; }

        public string AppName { get; }

        /// <summary>
        ///     Gets the path of the Windows AppData directory.
        /// </summary>
        public static string AppDataBaseDir => _generalAppDataPath
                                               ?? (_generalAppDataPath =
                                                   Environment.GetFolderPath(Environment.SpecialFolder
                                                       .ApplicationData));

        /// <summary>
        ///     Gets the path to this application's appdata folder.
        /// </summary>
        public string AppDataDir => _appDataPath
                                    ?? (_appDataPath = Path.Combine(AppDataBaseDir, CompanyName, AppName));

        public string SubDir(string name)
        {
            return Path.Combine(AppDataDir, name);
        }

        /// <summary>
        ///     Infers the company name and application name from the executing assembly.
        /// </summary>
        public CacheManager()
        {
            CompanyName = AssemblyUtils.CompanyName;
            AppName = AssemblyUtils.ProductName;
        }

        public CacheManager(string companyName, string appName)
        {
            CompanyName = companyName;
            AppName = appName;
        }

        /// <summary>
        ///     Save a file to the specified location.
        /// </summary>
        public void Save(string filename, byte[] content, bool overwrite = true)
        {
            string path = Path.Combine(AppDataDir, filename);
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
