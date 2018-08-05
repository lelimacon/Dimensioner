using System.Collections.Generic;
using System.Linq;
using Dimensioner.Components.Labels;

namespace Dimensioner.Components.Tables
{
    /// <summary>
    ///     An encapsulation of the XBRL "definitionNode" element
    ///     as defined in the table linkbase schema at https://www.xbrl.org/2014/table.xsd.
    ///     Specifications:
    ///     http://www.xbrl.org/Specification/table-linkbase/REC-2014-03-18/table-linkbase-REC-2014-03-18+corrected-errata-2016-03-09.html
    /// </summary>
    public abstract class DefinitionNode : TaxonomyComponent, ILabelized
    {
        public List<DefinitionNode> Children { get; set; }
        public XbrlLabels Labels { get; set; }

        // Accessors.
        public bool HasChildren => Children != null && Children.Any();

        public abstract int Depth { get; }
        public int Size => 1 + (Children?.Sum(arc => arc.Size) ?? 0);

        public int LeafCount => Children == null || !Children.Any()
            ? 1
            : Children.Sum(arc => arc.LeafCount);

        public string Header => Labels.Standard;
        public string RcCode => Labels.GetText(Label.RoleEnum.RcCode);

        internal DefinitionNode(XbrlSchema schema, string id)
            : base(schema, id)
        {
            Children = new List<DefinitionNode>();
        }
    }
}
