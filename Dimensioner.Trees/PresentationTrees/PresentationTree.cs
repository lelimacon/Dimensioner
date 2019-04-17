using System.Collections.Generic;
using Dimensioner.Components;
using Dimensioner.Components.Roles;

namespace Dimensioner.Trees.PresentationTrees
{
    public class PresentationTree : TaxonomyComponent
    {
        public Role Role { get; set; }
        public List<PresentationNode> Roots { get; set; }

        public PresentationTree(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
