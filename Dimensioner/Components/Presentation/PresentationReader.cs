using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Components.Calculations
{
    public class PresentationReader : LinkReader<PresentationLink, PresentationArc>
    {
        protected override string ReferenceRole => Ns.XbrlPresentationLinkbaseReferenceRole;

        public PresentationReader()
            : base("presentationLink", "presentationArc")
        {
        }

        protected override PresentationLink CreateLink(XbrlSchema schema, string id)
        {
            return new PresentationLink(schema, id);
        }

        protected override PresentationArc BuildArc(XElement n)
        {
            string order = n.Attr("order");
            string preferredLabel = n.Attr("preferredLabel");
            return new PresentationArc
            {
                Order = order == null ? 0 : double.Parse(order),
                PreferredLabel = preferredLabel,
            };
        }
    }
}
