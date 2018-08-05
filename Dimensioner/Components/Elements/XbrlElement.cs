using System;
using System.Xml.Linq;
using Dimensioner.Components.Labels;

namespace Dimensioner.Components.Elements
{
    public enum ElementType
    {
        Default,
        Unknown,
        Module,
        String,
        Documentation,
        Title,
        Locator,
        Arc,
        Resource,
        Extended,
        Simple,
        Decimal,
        NonZeroDecimal
    }

    public enum PeriodType
    {
        Default,
        Instant,
        Duration
    }

    public class ElementRaws
    {
        public XName Type { get; set; }
    }

    public class XbrlElement : TaxonomyComponent, ILabelized
    {
        public string Name { get; set; }
        public ElementType Type { get; set; }
        public bool Abstract { get; set; }
        public PeriodType Period { get; set; }
        public DateTime? CreationDate { get; set; }
        public ElementRaws Raws { get; set; }
        public XbrlLabels Labels { get; set; }

        public XbrlElement(XbrlSchema schema, string id)
            : base(schema, id)
        {
            Schema = schema;
            Id = id;
        }
    }
}
