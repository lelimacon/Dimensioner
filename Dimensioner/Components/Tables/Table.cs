using System.Collections.Generic;
using Dimensioner.Components.Labels;
using Dimensioner.Components.Roles;

namespace Dimensioner.Components.Tables
{
    public class Table : TaxonomyComponent, ILabelized
    {
        public Role Role { get; set; }
        public List<Breakdown> Breakdowns { get; set; }
        public XbrlLabels Labels { get; set; }

        public Table(XbrlSchema schema, string id)
            : base(schema, id)
        {
            Breakdowns = new List<Breakdown>();
        }
    }
}
