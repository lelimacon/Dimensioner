using System;
using System.Collections.Generic;
using System.Linq;
using Dimensioner.Components;

namespace Dimensioner
{
    public class XbrlSchemaSet : Modulable
    {
        private readonly Dictionary<string, XbrlSchema> _schemas;

        //public List<XbrlElement> Elements { get; set; }
        //public List<Linkbase> Linkbases { get; set; }

        // Accessors
        public IEnumerable<XbrlSchema> Schemas => _schemas.Select(s => s.Value);

        public XbrlSchemaSet()
        {
            _schemas = new Dictionary<string, XbrlSchema>();
        }

        public void Add(XbrlSchema schema)
        {
            if (_schemas.ContainsKey(schema.Path))
                throw new ArgumentException("Schema set already contains this schema.");
            _schemas[schema.Path] = schema;
        }

        public void Add(IEnumerable<XbrlSchema> schemas)
        {
            foreach (var schema in schemas)
                Add(schema);
        }

        public void Compile()
        {
            //Elements = _schemas.SelectMany(s => s.Value.Elements).ToList();
            //Linkbases = _schemas.SelectMany(s => s.Value.Linkbases).ToList();
        }

        public XbrlSchema Schema(string path)
        {
            return _schemas[path];
        }

        public XbrlSchema GetSchema(string path)
        {
            if (!_schemas.ContainsKey(path))
                return null;
            return _schemas[path];
        }

        /*
        public XbrlElement Element(Href href)
        {
            return Schema(href.AbsolutePath).Element(href.ResourceId);
        }

        public XbrlElement GetElement(Href href)
        {
            return GetSchema(href.AbsolutePath)?.GetElement(href.ResourceId);
        }
        */

        public T Component<T>(Href href)
            where T : ITaxonomyComponent
        {
            return Schema(href.AbsolutePath).Component<T>(href.ResourceId);
        }

        public ITaxonomyComponent Component(Href href)
        {
            return Schema(href.AbsolutePath).Component(href.ResourceId);
        }

        public T GetComponent<T>(Href href)
            where T : class, ITaxonomyComponent
        {
            return GetSchema(href.AbsolutePath)?.GetComponent<T>(href.ResourceId);
        }

        public ITaxonomyComponent GetComponent(Href href)
        {
            return GetSchema(href.AbsolutePath)?.GetComponent(href.ResourceId);
        }
    }
}
