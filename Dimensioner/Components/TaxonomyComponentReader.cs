using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Dimensioner.Components
{
    public class TaxonomyComponentReader
    {
        internal TaxonomyReader BaseReader { get; set; }

        /// <summary>
        ///     Adds an XBRL schema to the thread pool for reading.
        ///     Is meant to be accessed internally and by taxonomy component readers.
        /// </summary>
        /// <param name="path">The absolute path of the schema.</param>
        /// <returns>The empty shell of the future schema.</returns>
        protected XbrlSchema QueueSchema(string path)
        {
            if (!BaseReader.Reading)
                throw new Exception("Instances must be pushed while reading the taxonomy.");
            return BaseReader.Queue(null, path, null);
        }

        internal IEnumerable<TaxonomyComponent> Read(Modulable modulable, XDocument document)
        {
            return Read((dynamic) modulable, document);
        }

        internal IEnumerable<TaxonomyComponent> Read(XbrlSchemaSet schemaSet, XDocument document)
        {
            if (document != null)
                throw new ArgumentException("Specified XDocument must be null");
            return PostProcess(schemaSet);
        }

        public virtual IEnumerable<TaxonomyComponent> Read(XbrlSchema schema, XDocument document)
        {
            return null;
        }

        public virtual IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            return null;
        }

        public virtual IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            return null;
        }
    }
}
