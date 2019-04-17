using System.Xml.Linq;

namespace Dimensioner.Tables.DimensionFilter
{
    public class DimensionMember
    {
        public XName Name { get; set; }
        public string Linkrole { get; set; }
        public string Arcrole { get; set; }
        public string Axis { get; set; }
    }
}
