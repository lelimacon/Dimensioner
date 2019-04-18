using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Components;
using Dimensioner.Components.Arcroles;
using Dimensioner.Components.Elements;
using Dimensioner.Components.Roles;
using Dimensioner.Utils;

namespace Dimensioner.Trees.PresentationTrees
{
    public class PresentationTreeReader : TaxonomyComponentReader
    {
        private readonly ConcurrentDictionary<PresentationTree, TmpLocator> _arcs;

        public PresentationTreeReader()
        {
            _arcs = new ConcurrentDictionary<PresentationTree, TmpLocator>();
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            // Filter on definition linkbases.
            if (linkbase.Role != Ns.XbrlPresentationLinkbaseReferenceRole)
                return null;

            // Read and add definition shells.
            var presentations = document.Root.Children(Ns.Link, "presentationLink")
                .Select(n => BuildPresentation(linkbase, n)).ToList();

            // Register the new references to the taxonomy reader.
            var refs = _arcs.SelectMany(a => a.Value.Roots.Select(r => r.Locator.Href)).ToList();
            foreach (Href href in refs)
                QueueSchema(href.AbsolutePath);

            return new List<TaxonomyComponent>();
        }

        private PresentationTree BuildPresentation(Linkbase linkbase, XElement node)
        {
            string role = node.Attr(Ns.Xlink, "role");
            var roots = LocatorNode.ToLocatorGraph(linkbase.Path, node, false);

            // Create the definition.
            XbrlSchema schema = linkbase.Schema;
            var presentation = new PresentationTree(schema, $"{schema.Namespace}:{role}");
            _arcs[presentation] = new TmpLocator
            {
                RoleUri = role,
                Roots = roots.ToList()
            };
            return presentation;
        }

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Resolve locators and complete definitions.
            foreach (var e in _arcs)
            {
                PresentationTree presentation = e.Key;
                TmpLocator loc = e.Value;
                presentation.Roots = loc.Roots
                    .Select(l => ConvertLocator(schemaSet, l, null)).ToList();
                presentation.Role = schemaSet.GetComponent<Role>(loc.RoleUri);
            }

            return _arcs.Select(a => a.Key);
        }

        private PresentationNode ConvertLocator(XbrlSchemaSet schemaSet, LocatorNode loc, XArc arc)
        {
            var element = schemaSet.GetComponent<XbrlElement>(loc.Locator.Href);
            var node = new PresentationNode
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

        private PresentationArc ConvertArc(XbrlSchemaSet schemaSet, XArc arc, LocatorNode l)
        {
            return new PresentationArc
            {
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
