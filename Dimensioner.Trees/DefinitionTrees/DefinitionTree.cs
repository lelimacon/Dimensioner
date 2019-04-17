using System.Collections.Generic;
using Dimensioner.Components;
using Dimensioner.Components.Roles;

namespace Dimensioner.Trees.DefinitionTrees
{
    public class DefinitionTree : TaxonomyComponent
    {
        public Role Role { get; set; }
        public List<DefinitionNode> Roots { get; set; }

        public DefinitionTree(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
