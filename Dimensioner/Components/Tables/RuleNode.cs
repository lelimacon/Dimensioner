using System.Collections.Generic;
using System.Linq;
using Dimensioner.Components.Tables.Formula;

namespace Dimensioner.Components.Tables
{
    /// <summary>
    ///     An encapsulation of the XBRL "ruleNode" element
    ///     as defined in the table linkbase schema at https://www.xbrl.org/2014/table.xsd.
    ///     Specifications:
    ///     http://www.xbrl.org/Specification/table-linkbase/REC-2014-03-18/table-linkbase-REC-2014-03-18+corrected-errata-2016-03-09.html
    /// </summary>
    public class RuleNode : ClosedDefinitionNode
    {
        public Concept Concept { get; set; }
        public List<ExplicitDimension> ExplicitDimensions { get; set; }
        public bool Abstract { get; set; }
        public bool Merge { get; set; }

        // Accessors.
        public override int Depth => (Merge ? 0 : 1)
                                     + (Children?.Any() ?? false ? Children.Max(c => c.Depth) : 0);

        public int NonAbstractNodesCount
            => (Children?.OfType<RuleNode>()
                    .Sum(arc => arc.NonAbstractNodesCount) ?? 0) + (!Abstract || !HasChildren ? 1 : 0);

        public bool DirtyMerge => HasChildren && Id.EndsWith("root");

        internal RuleNode(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
