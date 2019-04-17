using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Components;
using Dimensioner.Components.Arcroles;
using Dimensioner.Components.Elements;
using Dimensioner.Components.Roles;
using Dimensioner.Utils;

namespace Dimensioner.Trees.CalculationTrees
{
    public class CalculationTreeReader : TaxonomyComponentReader
    {
        private readonly ConcurrentDictionary<CalculationTree, TmpLocator> _arcs;

        public CalculationTreeReader()
        {
            _arcs = new ConcurrentDictionary<CalculationTree, TmpLocator>();
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            // Filter on calculation linkbases.
            if (linkbase.Role != Ns.XbrlCalculationLinkbaseReferenceRole)
                return null;

            // Read and add calculation shells.
            var calculations = document.Root.Children(Ns.Link, "calculationLink")
                .Select(n => BuildCalculation(linkbase, n)).ToList();

            // Register the new references to the taxonomy reader.
            var refs = _arcs.SelectMany(a => a.Value.Roots.Select(r => r.Locator.Href)).ToList();
            foreach (Href href in refs)
                QueueSchema(href.AbsolutePath);

            return new List<TaxonomyComponent>();
        }

        private CalculationTree BuildCalculation(Linkbase linkbase, XElement node)
        {
            string role = node.Attr(Ns.Xlink, "role");
            var roots = LocatorNode.ToLocatorGraph(linkbase.Path, node, false);

            // Create the calculation.
            XbrlSchema schema = linkbase.Schema;
            var calculation = new CalculationTree(schema, $"{schema.Namespace}:{role}");
            _arcs[calculation] = new TmpLocator
            {
                RoleUri = role,
                Roots = roots.ToList()
            };
            return calculation;
        }

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Resolve locators and complete calculations.
            foreach (var e in _arcs)
            {
                CalculationTree calculation = e.Key;
                TmpLocator loc = e.Value;
                calculation.Roots = loc.Roots
                    .Select(l => ConvertLocator(schemaSet, l, null)).ToList();
                calculation.Role = schemaSet.GetComponent<Role>(loc.RoleUri);
            }

            return _arcs.Select(a => a.Key);
        }

        private CalculationNode ConvertLocator(XbrlSchemaSet schemaSet, LocatorNode loc, XArc arc)
        {
            var element = schemaSet.GetComponent<XbrlElement>(loc.Locator.Href);
            var node = new CalculationNode
            {
                Value = element,
                Children = loc.Children
                    .Select((l, i) => Pair(
                        ConvertArc(schemaSet, loc.ChildArcs[i], l),
                        ConvertLocator(schemaSet, l, arc)))
                    .ToList()
            };
            return node;
        }

        private CalculationArc ConvertArc(XbrlSchemaSet schemaSet, XArc arc, LocatorNode l)
        {
            string weightStr = arc.XNode.Attr("weight");
            return new CalculationArc
            {
                Weight = weightStr == null ? 0 : int.Parse(weightStr),
                Arcrole = schemaSet.GetComponent<Arcrole>(arc?.ArcRole)
            };
        }

        private static KeyValuePair<TKey, TValue> Pair<TKey, TValue>(TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        private class TmpLocator
        {
            public string RoleUri { get; set; }
            public List<LocatorNode> Roots { get; set; }
        }
    }
}
