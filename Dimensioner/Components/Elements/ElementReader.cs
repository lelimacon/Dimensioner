using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Components.Elements
{
    public class ElementReader : TaxonomyComponentReader
    {
        private readonly ConcurrentDictionary<string, XbrlElement> _elements;

        public ElementReader()
        {
            _elements = new ConcurrentDictionary<string, XbrlElement>();
        }

        public override IEnumerable<TaxonomyComponent> Read(XbrlSchema schema, XDocument document)
        {
            var elements = document.Root.Children(Ns.Xs, "element")
                .Select(n => ToElement(schema, n))
                .Where(e => e != null)
                .ToList();
            foreach (var element in elements)
                _elements[element.Id] = element;
            return elements;
        }

        private static XbrlElement ToElement(XbrlSchema schema, XElement node)
        {
            string id = node.Attr("id");
            if (id == null)
                return null;
            string type = node.Attr("type");
            string period = node.Attr(Ns.Xbrli, "periodType");
            string creationDateStr = node.Attr(Ns.Model, "creationDate");
            return new XbrlElement(schema, id)
            {
                Name = node.Attr("name"),
                Type = ToElementType(type),
                Abstract = node.Attr("abstract") == "true",
                Period = PeriodType.Default.TryParse(period),
                CreationDate = string.IsNullOrEmpty(creationDateStr)
                    ? null as DateTime?
                    : DateTime.Parse(creationDateStr),
                Raws = new ElementRaws
                {
                    Type = node.ToXName(type)
                }
            };
        }

        private static ElementType ToElementType(string type)
        {
            switch (type)
            {
                case null:
                    return ElementType.Default;
                case "model:moduleType":
                    return ElementType.Module;
                case "xbrli:stringItemType":
                    return ElementType.String;
                case "xl:documentationType":
                    return ElementType.Documentation;
                case "xl:titleType":
                    return ElementType.Title;
                case "xl:locatorType":
                    return ElementType.Locator;
                case "xl:arcType":
                    return ElementType.Arc;
                case "xl:resourceType":
                    return ElementType.Resource;
                case "xl:extendedType":
                    return ElementType.Extended;
                case "xl:simpleType":
                    return ElementType.Simple;
                case "decimal":
                    return ElementType.Decimal;
                case "xbrli:nonZeroDecimal":
                    return ElementType.NonZeroDecimal;
                default:
                    return ElementType.Unknown;
                //throw new IndexOutOfRangeException();
            }
        }
        
        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Return the arcroles (can also be found in the linkbases via the schemaSet).
            return _elements.Select(a => a.Value);
        }
    }
}
