using System.Collections.Generic;

namespace Dimensioner.Components.Arcroles
{
    public enum ArcroleCycles
    {
        Unknown,
        None,
        Undirected
    }

    public class Arcrole : TaxonomyComponent
    {
        /// <summary>
        ///     Corresponds to the link ID.
        /// </summary>
        public string Name { get; set; }

        public ArcroleCycles CyclesAllowed { get; set; }
        public string Definition { get; set; }
        public List<string> UsedOn { get; set; }

        // Accessors.
        public string Uri => Id;

        public Arcrole(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
