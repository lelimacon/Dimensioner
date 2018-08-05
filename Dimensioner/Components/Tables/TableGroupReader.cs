using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Components.Elements;
using Dimensioner.Components.Roles;
using Dimensioner.Utils;

namespace Dimensioner.Components.Tables
{
    public class TableGroupReader : TaxonomyComponentReader
    {
        private const string GroupArcrole = "http://www.eurofiling.info/xbrl/arcrole/group-table";
        private readonly ConcurrentDictionary<TableGroup, TmpLocator> _arcs;

        public TableGroupReader()
        {
            _arcs = new ConcurrentDictionary<TableGroup, TmpLocator>();
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            // Filter on generic linkbases.
            if (!string.IsNullOrEmpty(linkbase.Role))
                return null;
            bool hasLabelArcrole = document.Root
                .Children(Ns.Link, "arcroleRef")
                .Any(n => n.Attr("arcroleURI") == GroupArcrole);
            if (!hasLabelArcrole)
                return null;

            // Read and add table group shells.
            var components = document.Root.Children(Ns.Gen, "link")
                .Select(n => BuildTableGroup(linkbase, n))
                .Where(c => c != null)
                .ToList();

            // Register the new references to the taxonomy reader.
            var refs = _arcs.SelectMany(a => a.Value.Roots.Select(r => r.Locator.Href))
                .Where(href => href.DocumentUri.EndsWith(".xsd")).ToList();
            foreach (Href href in refs)
                QueueSchema(href.AbsolutePath);

            return components;
        }

        private TaxonomyComponent BuildTableGroup(Linkbase linkbase, XElement node)
        {
            string role = node.Attr(Ns.Xlink, "role");
            var roots = TaxonomyReader.ToLocatorGraph(linkbase.Path, node, true);

            // Create the definition.
            XbrlSchema schema = linkbase.Schema;
            var group = new TableGroup(schema, $"{schema.Namespace}:{role}");
            _arcs[group] = new TmpLocator
            {
                RoleUri = role,
                Roots = roots.ToList()
            };
            return group;
        }

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Resolve locators and complete definitions.
            foreach (var e in _arcs)
            {
                TableGroup group = e.Key;
                TmpLocator loc = e.Value;
                group.Roots = loc.Roots
                    .Select(l => ConvertLocator(schemaSet, l, null)).ToList();
                group.Role = schemaSet.GetComponent<Role>(loc.RoleUri);
            }

            return _arcs.Select(a => a.Key);
        }

        private TableGroupNode ConvertLocator(XbrlSchemaSet schemaSet, LocatorNode loc, Arc arc)
        {
            var element = schemaSet.GetComponent<XbrlElement>(loc.Locator.Href);
            var table = schemaSet.GetComponent<Table>(loc.Locator.Href);
            var node = new TableGroupNode
            {
                Value = new TableGroupValue {Element = element, Table = table},
                Children = loc.Children
                    .Select(l => ConvertLocator(schemaSet, l, arc))
                    .ToList()
            };
            return node;
        }

        private class TmpLocator
        {
            public string RoleUri { get; set; }
            public List<LocatorNode> Roots { get; set; }
        }
    }
}
