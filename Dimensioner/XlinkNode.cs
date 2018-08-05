using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner
{
    /// <summary>
    ///     An implementation for an XML node that supports XLink.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Xlink is dependent upon XML, and, for Gepsio, XLink will use the XML interfaces.
    ///         The XLinkNode class itself does not need to be interface based, because its implementation
    ///         will not need to change between platforms. It will simply leverage the lower-level
    ///         XML service layer, which is interface based.
    ///     </para>
    ///     <para>
    ///         This class implements the specification available at http://www.w3.org/TR/xlink11/.
    ///     </para>
    /// </remarks>
    public class XlinkNode
    {
        /// <summary>
        ///     An enumeration of possible types for an Xlink node.
        /// </summary>
        public enum XlinkType
        {
            /// <summary>
            ///     An unknown link type.
            /// </summary>
            Unknown,

            /// <summary>
            ///     The "simple" link type.
            /// </summary>
            Simple,

            /// <summary>
            ///     The "extended" link type.
            /// </summary>
            Extended,

            /// <summary>
            ///     The "locator" link type.
            /// </summary>
            Locator,

            /// <summary>
            ///     The "arc" link type.
            /// </summary>
            Arc,

            /// <summary>
            ///     The "resource" link type.
            /// </summary>
            Resource,

            /// <summary>
            ///     The "title" link type.
            /// </summary>
            Title
        }

        public XElement XNode { get; set; }

        /// <summary>
        ///     The type of Xlink node.
        /// </summary>
        public XlinkType Type { get; set; }

        /// <summary>
        ///     The value of the node's "type" attribute.
        /// </summary>
        public string TypeAttributeValue { get; set; }

        /// <summary>
        ///     The value of the node's "href" attribute. This value is the empty string
        ///     if the attribute was not found on the node.
        /// </summary>
        public Href Href { get; set; }

        /// <summary>
        ///     The value of the node's "role" attribute. This value is the empty string
        ///     if the attribute was not found on the node.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        ///     The value of the node's "arcrole" attribute. This value is the empty string
        ///     if the attribute was not found on the node.
        /// </summary>
        public string ArcRole { get; set; }

        /// <summary>
        ///     The value of the node's "title" attribute. This value is the empty string
        ///     if the attribute was not found on the node.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The value of the node's "show" attribute. This value is the empty string
        ///     if the attribute was not found on the node.
        /// </summary>
        public string Show { get; set; }

        /// <summary>
        ///     The value of the node's "actuate" attribute. This value is the empty string
        ///     if the attribute was not found on the node.
        /// </summary>
        public string Actuate { get; set; }

        /// <summary>
        ///     The value of the node's "label" attribute. This value is the empty string
        ///     if the attribute was not found on the node.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     The value of the node's "from" attribute. This value is the empty string
        ///     if the attribute was not found on the node.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        ///     The value of the node's "to" attribute. This value is the empty string
        ///     if the attribute was not found on the node.
        /// </summary>
        public string To { get; set; }

        internal static bool IsXlinkNode(XElement node)
        {
            var type = node.Attr(Ns.Xlink, "type");
            var href = node.Attr(Ns.Xlink, "href");
            return !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(href);
        }

        public XlinkNode()
        {
        }

        public XlinkNode(string basePath, XElement node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            XNode = node;
            TypeAttributeValue = node.Attr(Ns.Xlink, "type");
            Type = GetLinkType();
            var href = node.Attr(Ns.Xlink, "href");
            if (!string.IsNullOrEmpty(href))
                Href = new Href(basePath, href);
            Role = node.Attr(Ns.Xlink, "role");
            ArcRole = node.Attr(Ns.Xlink, "arcrole");
            Title = node.Attr(Ns.Xlink, "title");
            Show = node.Attr(Ns.Xlink, "show");
            Actuate = node.Attr(Ns.Xlink, "actuate");
            Label = node.Attr(Ns.Xlink, "label");
            From = node.Attr(Ns.Xlink, "from");
            To = node.Attr(Ns.Xlink, "to");
        }

        private XlinkType GetLinkType()
        {
            // According to section 4 of the XLink specification, if an element has an xlink:href attribute but does
            // not have an xlink:type attribute, then it is treated exactly as if it had an xlink:type attribute with
            // the value "simple".
            if (string.IsNullOrEmpty(TypeAttributeValue) && !string.IsNullOrEmpty(Href.ResourceId))
                return XlinkType.Simple;

            // At this point, we know that the node has a "type" attribute.
            return TypeAttributeValue.Switch(new Dictionary<string, XlinkType>
            {
                ["simple"] = XlinkType.Simple,
                ["extended"] = XlinkType.Extended,
                ["locator"] = XlinkType.Locator,
                ["arc"] = XlinkType.Arc,
                ["resource"] = XlinkType.Resource,
                ["title"] = XlinkType.Title
            }, XlinkType.Unknown);
        }
    }
}
