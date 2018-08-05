﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Dimensioner.Components.Arcroles;
using Dimensioner.Utils;

namespace Dimensioner.Components.Definitions
{
    public static class DefinitionWriter
    {
        private static readonly XNamespace Link = Ns.Link;
        private static readonly XNamespace Xlink = Ns.Xlink;

        public static string Write(this Definition definition, Stream output)
        {
            // Retrieve and convert elements to X nodes.
            var nodes = definition.Roots
                .SelectMany(n => n.Flatten)
                .ToList();
            var arcroles = nodes
                .Select(n => n.Arcrole)
                .Where(a => a != null).Distinct(a => a.Uri)
                .Select(BuildArcrole)
                .ToList();
            var elements = definition.Roots
                .SelectMany(n => BuildNodes(n, null))
                .ToList();

            // Create custom nodes.
            var linkNode = new XElement(Link + "definitionLink",
                new XAttribute(Xlink + "type", "locator"),
                new XAttribute(Xlink + "role", definition.Role.Uri),
                elements);
            var linkbaseNode = new XElement(Link + "linkbase",
                new XAttribute(XNamespace.Xmlns + "link", Link),
                new XAttribute(XNamespace.Xmlns + "xlink", Xlink),
                arcroles,
                linkNode);
            var document = new XDocument(
                new XDeclaration("1.0", "utf8", "no"),
                //new XProcessingInstruction("taxonomy-version", "2.3.0.0"),
                new XComment("XML file generated by Dimensioner"),
                linkbaseNode);

            // Write to file.
            document.Save(output);

            // Return the linkbase reference role.
            return Ns.XbrlDefinitionLinkbaseReferenceRole;
        }

        private static IEnumerable<XElement> BuildNodes(DefinitionNode node, DefinitionNode parent)
        {
            var locNode = new XElement(Link + "loc",
                new XAttribute(Xlink + "type", "locator"),
                new XAttribute(Xlink + "href", $"{node.Element?.Schema?.Path}#{node.Element?.Id}"),
                new XAttribute(Xlink + "label", $"loc_{node.Element?.Id}"));
            yield return locNode;
            for (int i = 0; i < node.Children.Count; i++)
            {
                //if (node.Element == null)
                //    continue;
                var child = node.Children[i];
                foreach (var n in BuildNodes(child, node))
                    yield return n;
                var arcNode = new XElement(Link + "definitionArc",
                    new XAttribute(Xlink + "type", "arc"),
                    new XAttribute(Xlink + "arcrole", child.Arcrole.Uri),
                    new XAttribute(Xlink + "from", $"loc_{node.Element?.Id}"),
                    new XAttribute(Xlink + "to", $"loc_{child.Element?.Id}"),
                    new XAttribute("order", i + 1));
                yield return arcNode;
            }
        }

        private static XElement BuildArcrole(Arcrole arcrole)
        {
            return new XElement(Link + "arcroleRef",
                new XAttribute("arcroleURI", arcrole.Uri),
                new XAttribute(Xlink + "type", "simple"),
                new XAttribute(Xlink + "href", $"{arcrole.Schema.Path}#{arcrole.Name}"));
        }
    }
}
