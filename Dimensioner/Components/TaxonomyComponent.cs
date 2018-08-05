namespace Dimensioner.Components
{
    public class TaxonomyComponent : ITaxonomyComponent
    {
        /// <summary>
        ///     Reference to the underlying schema.
        /// </summary>
        public XbrlSchema Schema { get; set; }

        /// <summary>
        ///     A *per component type* unique identifier.
        /// </summary>
        public string Id { get; set; }

        public TaxonomyComponent(XbrlSchema schema, string id)
        {
            Schema = schema;
            Id = id;
        }
    }
}
