using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Components.Arcroles;
using Dimensioner.Components.Elements;
using Dimensioner.Components.Roles;
using Dimensioner.Utils;

namespace Dimensioner.Components.Calculations
{
    public class CalculationReader : TaxonomyComponentReader
    {
        private readonly ConcurrentDictionary<CalculationLink, TmpLink> _links;
        private readonly ConcurrentDictionary<CalculationArc, TmpArc> _arcs;

        public CalculationReader()
        {
            _links = new ConcurrentDictionary<CalculationLink, TmpLink>();
            _arcs = new ConcurrentDictionary<CalculationArc, TmpArc>();
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            // Filter on calculation linkbases.
            if (linkbase.Role != Ns.XbrlCalculationLinkbaseReferenceRole)
                return null;

            // Read and add calculation shells.
            var calculationLinks = document.Root.Children(Ns.Link, "calculationLink")
                .Select(n => BuildLink(linkbase, n)).ToList();

            return calculationLinks;
        }

        private CalculationLink BuildLink(Linkbase linkbase, XElement node)
        {
            string role = node.Attr(Ns.Xlink, "role");

            // Fetch locators and arcs.
            var locs = node.Children(Ns.Link, "loc")
                .ToDictionary(n => n.Attr(Ns.Xlink, "label"), n => new Href(linkbase.Path, n.Attr(Ns.Xlink, "href")));
            var arcNodes = node.Children(Ns.Link, "calculationArc").ToList();
            var arcs = arcNodes.ToDictionary(BuildArc, n => BuildTmpArc(n, locs));

            // Register the new references to the taxonomy reader.
            var refs = locs.Select(l => l.Value).Distinct(h => h.AbsolutePath);
            foreach (Href href in refs)
                QueueSchema(href.AbsolutePath);

            // Create the link.
            XbrlSchema schema = linkbase.Schema;
            var link = new CalculationLink(schema, $"{schema.Namespace}:{role}")
            {
                Arcs = arcs.Select(a => a.Key).ToList(),
            };

            // Save temporary link and arcs.
            foreach (var arc in arcs)
                _arcs[arc.Key] = arc.Value;
            _links[link] = new TmpLink { RoleUri = role };

            return link;
        }

        private static CalculationArc BuildArc(XElement n)
        {
            string weight = n.Attr("weight");
            string order = n.Attr("order");
            return new CalculationArc
            {
                Weight = weight == null ? 0 : int.Parse(weight),
                Order = weight == null ? 0 : double.Parse(order),
            };
        }

        private static TmpArc BuildTmpArc(XElement n, Dictionary<string, Href> locs)
        {
            return new TmpArc
            {
                Arcrole = n.Attr(Ns.Xlink, "role"),
                From = locs[n.Attr(Ns.Xlink, "from")],
                To = locs[n.Attr(Ns.Xlink, "to")],
            };
        }

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Resolve locators.
            foreach (var e in _links)
            {
                CalculationLink link = e.Key;
                TmpLink tmpLink = e.Value;
                link.Role = schemaSet.GetComponent<Role>(tmpLink.RoleUri);
            }
            foreach (var e in _arcs)
            {
                CalculationArc arc = e.Key;
                TmpArc tmpArc = e.Value;
                arc.Arcrole = schemaSet.GetComponent<Arcrole>(tmpArc.Arcrole);
                arc.From = schemaSet.GetComponent<XbrlElement>(tmpArc.From);
                arc.To = schemaSet.GetComponent<XbrlElement>(tmpArc.To);
            }

            return _links.Select(l => l.Key);
        }

        private class TmpArc
        {
            public string Arcrole { get; set; }
            public Href From { get; set; }
            public Href To { get; set; }
        }

        private class TmpLink
        {
            public string RoleUri { get; set; }
        }
    }
}
