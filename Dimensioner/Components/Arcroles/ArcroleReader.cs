using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Components.Arcroles
{
    public class ArcroleReader : TaxonomyComponentReader
    {
        private readonly ConcurrentDictionary<string, Arcrole> _arcroles;

        public ArcroleReader()
        {
            _arcroles = new ConcurrentDictionary<string, Arcrole>();
        }

        public override IEnumerable<TaxonomyComponent> Read(XbrlSchema schema, XDocument document)
        {
            var arcroles = document.Root.Children(Ns.Xs, "annotation")
                .SelectMany(n => n.Children(Ns.Xs, "appinfo"))
                .SelectMany(n => n.Children(Ns.Link, "arcroleType"))
                .Select(n => ReadArcrole(schema, n))
                .ToList();
            return arcroles;
        }

        private Arcrole ReadArcrole(XbrlSchema schema, XElement node)
        {
            string id = node.Attr("id");
            string uri = node.Attr("arcroleURI");

            Arcrole arcrole;
            lock (_arcroles)
            {
                if (_arcroles.ContainsKey(uri))
                {
                    // Retrieve the existing arcrole shell.
                    arcrole = _arcroles[uri];
                    if (arcrole.Name != null)
                    {
                        Console.WriteLine($"Old arcrole = {arcrole.Id} ({arcrole.Name})");
                        Console.WriteLine($"- Schema {arcrole.Schema.Path}");
                        Console.WriteLine($"New arcrole = {uri} ({id})");
                        Console.WriteLine($"- Schema {schema.Path}");
                        throw new Exception("An arcrole with this uri already exists");
                    }
                    //throw new Exception("An arcrole with this uri already exists");
                }
                else
                {
                    // Create and add the arcrole to the dictionary (as soon as possible since its async).
                    arcrole = new Arcrole(schema, uri);
                    _arcroles[uri] = arcrole;
                }
            }

            // Fill in the remaining properties.
            arcrole.Name = id;
            arcrole.Schema = schema;
            arcrole.CyclesAllowed = ArcroleCycles.Unknown.TryParse(node.Attr("cyclesAllowed"));
            arcrole.Definition = node.GetChild(Ns.Link, "definition")?.Value;
            arcrole.UsedOn = node.Children(Ns.Link, "usedOn").Select(n => n.Value).ToList();

            return arcrole;
        }

        public override IEnumerable<TaxonomyComponent> Read(Linkbase linkbase, XDocument document)
        {
            var arcroles = document.Root.Children(Ns.Link, "arcroleRef")
                .Select(n => ReadArcroleRef(linkbase, n))
                .ToList();
            return arcroles;
        }

        private TaxonomyComponent ReadArcroleRef(Linkbase linkbase, XElement node)
        {
            string uri = node.Attr("arcroleURI");
            var href = new Href(linkbase.Path, node.Attr(Ns.Xlink, "href"));
            string id = href.ResourceId;

            // Queue schema where arcrole is defined.
            XbrlSchema schema = QueueSchema(href.AbsolutePath);

            Arcrole arcrole;
            lock (_arcroles)
            {
                if (_arcroles.ContainsKey(uri))
                {
                    // Retrieve the existing arcrole shell.
                    arcrole = _arcroles[uri];
                }
                else
                {
                    // Create and add the arcrole to the dictionary.
                    arcrole = new Arcrole(schema, uri);
                    _arcroles[uri] = arcrole;
                }
            }

            return arcrole;
        }

        public override IEnumerable<TaxonomyComponent> PostProcess(XbrlSchemaSet schemaSet)
        {
            // Return the arcroles (can also be found in the linkbases via the schemaSet).
            return _arcroles.Select(a => a.Value);
        }
    }
}
