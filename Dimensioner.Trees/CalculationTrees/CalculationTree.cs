using System.Collections.Generic;
using Dimensioner.Components;
using Dimensioner.Components.Roles;

namespace Dimensioner.Trees.CalculationTrees
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
