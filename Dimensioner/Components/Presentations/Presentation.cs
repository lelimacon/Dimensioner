using System.Collections.Generic;
using Dimensioner.Components.Roles;

namespace Dimensioner.Components.Presentations
{
    public class Presentation : TaxonomyComponent
    {
        public Role Role { get; set; }
        public List<PresentationNode> Roots { get; set; }

        public Presentation(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
