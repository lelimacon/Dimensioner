namespace Dimensioner.Components
{
    public interface ITaxonomyComponent
    {
        /// <summary>
        ///     Reference to the underlying schema.
        /// </summary>
        XbrlSchema Schema { get; set; }

        /// <summary>
        ///     A *per component type* unique identifier.
        /// </summary>
        string Id { get; set; }
    }
}
