using System.Collections.Generic;
using Dimensioner.Components;
using Dimensioner.Components.Labels;

namespace Dimensioner.Tables
{
    public class Breakdown : TaxonomyComponent, ILabelized
    {
        public Table Table { get; set; }

        // Properties set by parent.
        public List<DefinitionNode> Nodes { get; set; }

        public XbrlLabels Labels { get; set; }

        // Properties retrieved in node.
        public string ParentChildOrder { get; }

        // Accessors.
        public string Header => Labels.Standard;

        /// <summary>
        ///     An encapsulation of the XBRL "breakdown" element
        ///     as defined in the table linkbase schema at https://www.xbrl.org/2014/table.xsd
        ///     Specifications:
        ///     http://www.xbrl.org/Specification/table-linkbase/REC-2014-03-18/table-linkbase-REC-2014-03-18+corrected-errata-2016-03-09.html
        /// </summary>
        internal Breakdown(XbrlSchema schema, string id)
            : base(schema, id)
        {
            //Id = node.GetAttributeValue("id");
            //ParentChildOrder = node.GetAttributeValue("parentChildOrder");
            Nodes = new List<DefinitionNode>();
        }
    }
}
