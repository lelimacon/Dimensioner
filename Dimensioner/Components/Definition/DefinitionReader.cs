using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Components.Calculations
{
    public class DefinitionReader : LinkReader<DefinitionLink, DefinitionArc>
    {
        protected override string ReferenceRole => Ns.XbrlDefinitionLinkbaseReferenceRole;

        public DefinitionReader()
            : base("definitionLink", "definitionArc")
        {
        }

        protected override DefinitionLink CreateLink(XbrlSchema schema, string id)
        {
            return new DefinitionLink(schema, id);
        }

        protected override DefinitionArc BuildArc(XElement n)
        {
            string order = n.Attr("order");
            return new DefinitionArc
            {
                Order = order == null ? 0 : double.Parse(order),
            };
        }
    }
}
