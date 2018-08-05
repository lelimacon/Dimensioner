using System.Xml.Linq;

namespace Dimensioner.Components.Tables.DimensionFilter
{
    public class ExplicitDimension : FilterVariable
    {
        public XName Dimension { get; set; }
        public DimensionMember Member { get; set; }
    }
}
