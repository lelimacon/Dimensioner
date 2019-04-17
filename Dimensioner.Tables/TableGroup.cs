using System.Collections.Generic;
using Dimensioner.Components;
using Dimensioner.Components.Roles;

namespace Dimensioner.Tables
{
    public class TableGroup : TaxonomyComponent
    {
        public Role Role { get; set; }
        public List<TableGroupNode> Roots { get; set; }

        public TableGroup(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
