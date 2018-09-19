using System.Collections.Generic;
using Dimensioner.Components.Roles;

namespace Dimensioner.Components.CalculationTrees
{
    public class CalculationTree : TaxonomyComponent
    {
        public Role Role { get; set; }
        public List<CalculationNode> Roots { get; set; }

        public CalculationTree(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
