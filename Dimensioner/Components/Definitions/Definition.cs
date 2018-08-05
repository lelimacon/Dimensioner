using System.Collections.Generic;
using Dimensioner.Components.Roles;

namespace Dimensioner.Components.Definitions
{
    public class Definition : TaxonomyComponent
    {
        public Role Role { get; set; }
        public List<DefinitionNode> Roots { get; set; }

        public Definition(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
