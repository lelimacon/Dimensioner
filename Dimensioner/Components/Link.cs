using Dimensioner.Components.Roles;
using System.Collections.Generic;

namespace Dimensioner.Components
{
    public class Link<TArc> : TaxonomyComponent
        where TArc : Arc
    {
        public Role Role { get; set; }
        public List<TArc> Arcs { get; set; }

        public Link(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
