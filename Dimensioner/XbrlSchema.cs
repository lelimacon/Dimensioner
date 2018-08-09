using System.Collections.Generic;
using Dimensioner.Components;

namespace Dimensioner
{
    public class XbrlSchema : Modulable
    {
        public string Path { get; set; }
        public string Namespace { get; set; }
        public List<XbrlSchema> Children { get; set; }

        internal XbrlSchema(string path)
        {
            Path = path;
        }

        internal XbrlSchema(string path, string targetNamespace)
        {
            Path = path;
            Namespace = targetNamespace;
        }
    }
}
