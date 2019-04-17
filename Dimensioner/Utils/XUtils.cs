using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Dimensioner.Utils
{
    public static class XUtils
    {
        public static XName ToXName(this XElement node, string name)
        {
            if (name == null)
                return null;
            if (name.Contains(":"))
            {
                var nsn = name.Split(':');
                XNamespace ns = node.GetNamespaceOfPrefix(nsn[0]);
                string localName = nsn[1];
                return ns + localName;
            }
            return name;
        }

        /// <summary>
        ///     Retrieves a single child node (XElement).
        ///     The search is not recursive.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="name">The name of the attribute, with or without the namespace.</param>
        /// <returns>The child node.</returns>
        public static XElement Child(this XElement node, string name)
        {
            return node.Children(name).Single();
        }

        /// <summary>
        ///     Retrieves a single child node (XElement).
        ///     The search is not recursive.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="name">The name of the attribute, with or without the namespace.</param>
        /// <returns>The child node.</returns>
        public static XElement Child(this XElement node, string ns, string name)
        {
            return node.Children(ns, name).Single();
        }

        /// <summary>
        ///     Retrieves a single child node (XElement).
        ///     The search is not recursive.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="name">The name of the attribute, with or without the namespace.</param>
        /// <returns>The child node.</returns>
        public static XElement GetChild(this XElement node, string name)
        {
            return node.Children(name).SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves a single child node (XElement).
        ///     The search is not recursive.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="name">The name of the attribute, with or without the namespace.</param>
        /// <returns>The child node.</returns>
        public static XElement GetChild(this XElement node, string ns, string name)
        {
            return node.Children(ns, name).SingleOrDefault();
        }

        /// <summary>
        ///     Retrieves the next level child nodes (XElement).
        ///     The search is not recursive.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="name">The name of the attribute, with or without the namespace.</param>
        /// <returns>The child nodes.</returns>
        public static IEnumerable<XElement> Children(this XElement node, string name)
        {
            if (name.Contains(":"))
            {
                var nsn = name.Split(':');
                return node.Children(nsn[1], nsn[0]);
            }
            if (name.Contains("{") && name.Contains("}"))
                return node.Elements(name);
            return node.Elements().Where(a => a.Name.LocalName == name);
        }

        /// <summary>
        ///     Retrieves the next level child nodes (XElement).
        ///     The search is not recursive.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="ns">The namespace of the attribute.</param>
        /// <returns>The child nodes.</returns>
        public static IEnumerable<XElement> Children(this XElement node, string ns, string localName)
        {
            XName xname = (XNamespace) ns + localName;
            return node.Elements(xname);
        }

        /// <summary>
        ///     Retrieves the next level child nodes (XElement).
        ///     The search is not recursive.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="ns">The namespaces of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <returns>The child nodes.</returns>
        public static IEnumerable<XElement> Children(this XElement node, IEnumerable<string> ns, string localName)
        {
            return ns.SelectMany(n => node.Children(n, localName));
        }

        /// <summary>
        ///     Retrieves an attribute value.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="name">The name of the attribute, with or without the namespace.</param>
        /// <returns>The attribute value, or null if not found.</returns>
        public static string Attr(this XElement node, string name)
        {
            if (name.Contains(":"))
            {
                var nsn = name.Split(':');
                return node.Attr(nsn[1], nsn[0]);
            }
            if (name.Contains("{") && name.Contains("}"))
                return node.Attribute(name)?.Value;
            return node.Attributes()
                .SingleOrDefault(a => a.Name.LocalName == name)?.Value;
        }

        /// <summary>
        ///     Retrieves an attribute value.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="ns">The namespace of the attribute.</param>
        /// <returns>The attribute value, or null if not found.</returns>
        public static string Attr(this XElement node, string ns, string localName)
        {
            XName xname = (XNamespace) ns + localName;
            return node.Attribute(xname)?.Value;
        }

        /// <summary>
        ///     Sets an attribute value.
        /// </summary>
        /// <param name="node">The XML element.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="ns">The namespace of the attribute.</param>
        /// <param name="value">The attribute new value.</param>
        /// <returns>The attribute value, or null if not found.</returns>
        public static void SetAttr(this XElement node, string ns, string localName, object value)
        {
            XName xname = (XNamespace) ns + localName;
            node.SetAttributeValue(xname, value);
        }
    }
}
