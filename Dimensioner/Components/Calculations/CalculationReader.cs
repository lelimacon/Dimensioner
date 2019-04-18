using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Components.Calculations
{
    public class CalculationReader : LinkReader<CalculationLink, CalculationArc>
    {
        protected override string ReferenceRole => Ns.XbrlCalculationLinkbaseReferenceRole;

        public CalculationReader()
            : base("calculationLink", "calculationArc")
        {
        }

        protected override CalculationLink CreateLink(XbrlSchema schema, string id)
        {
            return new CalculationLink(schema, id);
        }

        protected override CalculationArc BuildArc(XElement n)
        {
            string order = n.Attr("order");
            string weight = n.Attr("weight");
            return new CalculationArc
            {
                Order = order == null ? 0 : double.Parse(order),
                Weight = weight == null ? 0 : int.Parse(weight),
            };
        }
    }
}
