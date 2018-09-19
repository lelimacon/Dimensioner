using System.Collections.Generic;
using Dimensioner.Components.Roles;

namespace Dimensioner.Components.Calculations
{
    public class CalculationLink : TaxonomyComponent
    {
        public Role Role { get; set; }
        public List<CalculationArc> Arcs { get; set; }

        public CalculationLink(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
