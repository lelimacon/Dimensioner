using System.Collections.Generic;
using Dimensioner.Components.Roles;

namespace Dimensioner.Components.Calculations
{
    public class Calculation : TaxonomyComponent
    {
        public Role Role { get; set; }
        public List<CalculationNode> Roots { get; set; }

        public Calculation(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
