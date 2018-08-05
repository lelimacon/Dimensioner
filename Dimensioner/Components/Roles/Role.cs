using System.Collections.Generic;

namespace Dimensioner.Components.Roles
{
    public class Role : TaxonomyComponent
    {
        /// <summary>
        ///     Corresponds to the link ID.
        /// </summary>
        public string Name { get; set; }

        public string Definition { get; set; }
        public List<string> UsedOn { get; set; }

        // Accessors
        public string Uri => Id;

        public Role(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
