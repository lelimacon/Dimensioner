using System.Xml.Linq;

namespace Dimensioner.Tables
{
    public class DimensionAspect : Aspect
    {
        public XName DimensionName { get; }

        public DimensionAspect(XName dimensionName)
        {
            DimensionName = dimensionName;
        }
    }
}
