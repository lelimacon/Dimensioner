using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Components;
using Dimensioner.Components.Arcroles;
using Dimensioner.Components.Elements;
using Dimensioner.Components.Roles;
using Dimensioner.Utils;

namespace Dimensioner.Trees.DefinitionTrees
{
    public class DefinitioTreeReader : TaxonomyComponentReader
    {
        //private Definitions _definitions;
        private readonly ConcurrentDictionary<DefinitionTree, TmpLocator> _arcs;

        public DefinitioTreeReader()
        {
            //_definitions = new Definitions();
            _arcs = new ConcurrentDictionary<DefinitionTree, TmpLocator>();
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            // Filter on definition linkbases.
            if (linkbase.Role != Ns.XbrlDefinitionLinkbaseReferenceRole)
                return null;

            // Read and add definition shells.
            var definitions = document.Root.Children(Ns.Link, "definitionLink")
                .Select(n => BuildDefinition(linkbase, n)).ToList();

            // Register the new references to the taxonomy reader.
            var refs = _arcs.SelectMany(a => a.Value.Roots.Select(r => r.Locator.Href)).ToList();
            foreach (Href href in refs)
                QueueSchema(href.AbsolutePath);

            return new List<TaxonomyComponent>();
        }

        private DefinitionTree BuildDefinition(Linkbase linkbase, XElement node)
        {
            string role = node.Attr(Ns.Xlink, "role");
            var roots = LocatorNode.ToLocatorGraph(linkbase.Path, node, false);

            // Create the definition.
            XbrlSchema schema = linkbase.Schema;
            var definition = new DefinitionTree(schema, $"{schema.Namespace}:{role}");
            _arcs[definition] = new TmpLocator
            {
                RoleUri = role,
                Roots = roots.ToList()
            };
            return definition;
        }

        /*
        private static DefinitionArc ConvertArc(Linkbase linkbase, XElement n)
        {
            var arc = new DefinitionArc(linkbase.Path, n);
            string role = arc.ArcRole;
            return arc;
        }
        */

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Resolve locators and complete definitions.
            foreach (var e in _arcs)
            {
                DefinitionTree definition = e.Key;
                TmpLocator loc = e.Value;
                definition.Roots = loc.Roots
                    .Select(l => ConvertLocator(schemaSet, l, null)).ToList();
                definition.Role = schemaSet.GetComponent<Role>(loc.RoleUri);
            }

            return _arcs.Select(a => a.Key);
        }

        private DefinitionNode ConvertLocator(XbrlSchemaSet schemaSet, LocatorNode loc, XArc arc)
        {
            var element = schemaSet.GetComponent<XbrlElement>(loc.Locator.Href);
            var arcrole = schemaSet.GetComponent<Arcrole>(arc?.ArcRole);
            var node = new DefinitionNode
            {
                Element = element,
                Arcrole = arcrole,
                //Label = null,
                Order = arc?.Order,
                Children = loc.Children
                    .Select((l, i) => ConvertLocator(schemaSet, l, loc.ChildArcs[i]))
                    .ToList()
            };
            return node;
        }


        private class TmpLocator
        {
            public string RoleUri { get; set; }
            public List<LocatorNode> Roots { get; set; }
        }

        /*
        private class DefinitionArc : XlinkNode
        {
            public int Order { get; set; }

            public DefinitionArc(string basePath, XElement node)
                : base(basePath, node)
            {
                string orderStr = node.Attr(Ns.Xlink, "order");
                Order = orderStr == null ? 0 : int.Parse(orderStr);
            }
        }
        */
    }
}
