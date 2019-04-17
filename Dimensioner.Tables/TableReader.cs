using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Components;
using Dimensioner.Components.Roles;
using Dimensioner.Tables.DimensionFilter;
using Dimensioner.Tables.Formula;
using Dimensioner.Utils;

namespace Dimensioner.Tables
{
    public class TableReader : TaxonomyComponentReader
    {
        private readonly ConcurrentDictionary<string, TmpTable> _tables;

        public TableReader()
        {
            _tables = new ConcurrentDictionary<string, TmpTable>();
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            // Filter on generic linkbases.
            if (!string.IsNullOrEmpty(linkbase.Role))
                return null;

            // Read and add definition shells.
            var components = document.Root.Children(Ns.Gen, "link")
                .SelectMany(n => BuildTable(linkbase, n))
                .Where(c => c != null)
                .ToList();

            return components;
        }

        private IEnumerable<TaxonomyComponent> BuildTable(Linkbase linkbase, XElement node)
        {
            string role = node.Attr(Ns.Xlink, "role");
            XbrlSchema schema = linkbase.Schema;

            // Read table nodes.
            var tables = node
                .Children(Ns.XbrlTableLinkbase, "table")
                .ToDictionary(n => n.Attr(Ns.Xlink, "label"), n => new Table(schema, n.Attr("id")));

            if (!tables.Any())
                return new List<TaxonomyComponent>();

            // Read arcs.
            var aspectNodeFilterArcs = node
                .Children(Ns.XbrlTableLinkbase, "aspectNodeFilterArc")
                .Select(n => new XlinkNode(linkbase.Path, n))
                .ToList();
            var subtreeArcs = node
                .Children(Ns.XbrlTableLinkbase, "definitionNodeSubtreeArc")
                .Select(n => new XlinkNode(linkbase.Path, n))
                .ToList();
            var breakdownTreeArcs = node
                .Children(Ns.XbrlTableLinkbase, "breakdownTreeArc")
                .Select(n => new XlinkNode(linkbase.Path, n))
                .ToList();
            var tableBreakdownArcs = node
                .Children(Ns.XbrlTableLinkbase, "tableBreakdownArc")
                .Select(n => new XlinkNode(linkbase.Path, n))
                .ToList();

            // Read elements.
            // TODO: Add other filter variables.
            var filterVariables = node
                .Children(Ns.XbrlFilterDimension, "explicitDimension")
                .ToDictionary(n => n.Attr(Ns.Xlink, "label"), ToExplicitFilterVariable);
            var rNodes = node
                .Children(Ns.XbrlTableLinkbase, "ruleNode")
                .Select(n => (n.Attr(Ns.Xlink, "label"), CreateRuleNode(n, schema)));
            var aNodes = node
                .Children(Ns.XbrlTableLinkbase, "aspectNode")
                .Select(n => (n.Attr(Ns.Xlink, "label"), CreateAspectNode(n, schema)));
            var definitionNodes = aNodes.Concat(rNodes)
                .ToDictionary(n => n.Item1, n => n.Item2);
            var breakdowns = node
                .Children(Ns.XbrlTableLinkbase, "breakdown")
                .ToDictionary(n => n.Attr(Ns.Xlink, "label"), n => new Breakdown(schema, n.Attr("id")));

            foreach (var arc in tableBreakdownArcs)
            {
                var table = tables[arc.From];
                var breakdown = breakdowns[arc.To];
                table.Breakdowns.Add(breakdown);
                breakdown.Table = table;
            }
            foreach (var arc in breakdownTreeArcs)
                breakdowns[arc.From].Nodes.Add(definitionNodes[arc.To]);
            foreach (var arc in subtreeArcs)
                definitionNodes[arc.From].Children.Add(definitionNodes[arc.To]);
            foreach (var arc in aspectNodeFilterArcs)
                (definitionNodes[arc.From] as AspectNode)?.FilterVariables?.Add(filterVariables[arc.To]);

            // Add temporary tables for rule assignment.
            foreach (var table in tables)
                _tables[table.Value.Id] = new TmpTable
                {
                    RoleUri = role,
                    Table = table.Value
                };

            // Return all components.
            return tables.Select(t => t.Value)
                .Concat(breakdowns.Select(d => d.Value).Cast<TaxonomyComponent>())
                .Concat(definitionNodes.Select(d => d.Value).Cast<TaxonomyComponent>());
        }

        private static DimensionFilter.ExplicitDimension ToExplicitFilterVariable(XElement node)
        {
            var memberNode = node.Child(Ns.XbrlFilterDimension, "member");
            var member = new DimensionMember
            {
                Name = memberNode.ToXName(memberNode.Child(Ns.XbrlFilterDimension, "qname").Value),
                Linkrole = memberNode.Child(Ns.XbrlFilterDimension, "linkrole").Value,
                Arcrole = memberNode.Child(Ns.XbrlFilterDimension, "arcrole").Value,
                Axis = memberNode.Child(Ns.XbrlFilterDimension, "axis").Value
            };
            return new DimensionFilter.ExplicitDimension
            {
                // TODO: Include omit element.
                Dimension = node.ToXName(node.Child(Ns.XbrlFilterDimension, "dimension").Value),
                Member = member
            };
        }

        private static DefinitionNode CreateRuleNode(XElement node, XbrlSchema schema)
        {
            var dimensions = node
                .Children(Ns.XbrlFormula, "explicitDimension")
                .Select(n => new Formula.ExplicitDimension
                {
                    // TODO: Include omit element.
                    Member = new Member(n.Child(Ns.XbrlFormula, "member")),
                    Dimension = n.ToXName(n.Attr("dimension"))
                });
            var conceptNode = node.GetChild(Ns.XbrlFormula, "concept");
            Concept concept = conceptNode == null
                ? null
                : new Concept
                {
                    QName = node.ToXName(conceptNode.Child(Ns.XbrlFormula, "qname").Value)
                };
            var result = new RuleNode(schema, node.Attr("id"))
            {
                Concept = concept,
                ExplicitDimensions = dimensions.ToList(),
                Abstract = node.Attr("abstract") == "true",
                Merge = node.Attr("merge") == "true"
            };
            return result;
        }

        private static DefinitionNode CreateAspectNode(XElement node, XbrlSchema schema)
        {
            var dimensionAspect = node
                .Children(Ns.XbrlTableLinkbase, "dimensionAspect")
                .SingleOrDefault();
            var periodAspect = node
                .Children(Ns.XbrlTableLinkbase, "periodAspect")
                .SingleOrDefault();
            Aspect aspect;
            if (dimensionAspect != null)
                aspect = new DimensionAspect(node.ToXName(dimensionAspect.Value));
            else if (periodAspect != null)
                aspect = new PeriodAspect();
            else
                throw new Exception("Unknown aspect in node");
            var result = new AspectNode(schema, node.Attr("id"))
            {
                Aspect = aspect
            };
            return result;
        }

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Assign the roles.
            foreach (var table in _tables)
                table.Value.Table.Role = schemaSet.Component<Role>(table.Value.RoleUri);

            return _tables.Select(t => t.Value.Table);
        }

        private class TmpTable
        {
            public string RoleUri { get; set; }
            public Table Table { get; set; }
        }
    }
}
