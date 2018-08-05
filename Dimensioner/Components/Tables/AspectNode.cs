using System.Collections.Generic;
using System.Linq;

namespace Dimensioner.Components.Tables
{
    /// <summary>
    ///     An encapsulation of the XBRL "aspectNode" element
    ///     as defined in the table linkbase schema at https://www.xbrl.org/2014/table.xsd.
    ///     Specifications:
    ///     http://www.xbrl.org/Specification/table-linkbase/REC-2014-03-18/table-linkbase-REC-2014-03-18+corrected-errata-2016-03-09.html
    /// </summary>
    public class AspectNode : OpenDefinitionNode
    {
        public List<FilterVariable> FilterVariables { get; set; }
        public Aspect Aspect { get; set; }

        // Accessors.
        public override int Depth => Children?.Any() ?? false ? Children.Max(c => c.Depth) : 0;

        internal AspectNode(XbrlSchema schema, string id)
            : base(schema, id)
        {
            FilterVariables = new List<FilterVariable>();
        }
    }
}
