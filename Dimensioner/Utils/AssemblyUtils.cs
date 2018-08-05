using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Dimensioner.Utils
{
    public class AssemblyUtils
    {
        private static Assembly _baseAssembly;
        private static FileVersionInfo _assemblyInfo;
        private static string _assemblyName;
        private static string _assemblyPath;
        private static string _assemblyFolder;
        private static string _productVersion;
        private static string _companyName;
        private static string _productName;

        private static Assembly BaseAssembly
        {
            get
            {
                if (_baseAssembly == null)
                    try
                    {
                        _baseAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
                    }
                    catch (Exception)
                    {
                        _baseAssembly = Assembly.GetCallingAssembly();
                    }
                return _baseAssembly;
            }
        }

        private static FileVersionInfo AssemblyInfo => _assemblyInfo ??
                                                       (_assemblyInfo =
                                                           FileVersionInfo.GetVersionInfo(BaseAssembly.Location));

        public static string AssemblyName => _assemblyName ??
                                             (_assemblyName = BaseAssembly.GetName().Name);

        public static string AssemblyPath => _assemblyPath ??
                                             (_assemblyPath = BaseAssembly.Location);

        public static string AssemblyFolder => _assemblyFolder ??
                                               (_assemblyFolder = Path.GetDirectoryName(AssemblyPath));

        public static string ProductVersion => _productVersion ??
                                               (_productVersion = AssemblyInfo.ProductVersion);

        public static string CompanyName => _companyName ??
                                            (_companyName = AssemblyInfo.CompanyName);

        public static string ProductName => _productName ??
                                            (_productName = AssemblyInfo.ProductName);
    }
}
