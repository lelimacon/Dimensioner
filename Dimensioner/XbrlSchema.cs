using System.Collections.Generic;
using Dimensioner.Components;

namespace Dimensioner
{
    public class XbrlSchema : Modulable
    {
        public string Path { get; set; }
        public string Namespace { get; set; }

        public List<XbrlSchema> Children { get; set; }
        //public List<XbrlElement> Elements { get; set; }
        //public List<Linkbase> Linkbases { get; set; }

        internal XbrlSchema(string path)
        {
            Path = path;
        }

        internal XbrlSchema(string path, string targetNamespace)
        {
            Path = path;
            Namespace = targetNamespace;
        }

        /*
        public XbrlElement Element(string id)
        {
            return Elements.Single(e => e.Id == id);
        }

        public XbrlElement GetElement(string id)
        {
            return Elements.SingleOrDefault(e => e.Id == id);
        }
        */
    }
}
