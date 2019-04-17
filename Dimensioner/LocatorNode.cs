using System.Collections.Generic;
using System.Xml.Linq;

namespace Dimensioner
{
    public class LocatorNode
    {
        public List<LocatorNode> Children { get; set; }

        public List<LocatorNode> Parents { get; set; }

        //public LocatorNode Parent { get; set; }
        public XlinkNode Locator { get; set; }

        /// <summary>
        ///     The arc to the child locator.
        /// </summary>
        public List<XArc> ChildArcs { get; set; }

        /*
        /// <summary>
        ///     The arc to the parent locator.
        /// </summary>
        public List<XElement> ParentArcs { get; set; }
        //public XElement Arc { get; set; }
        */

        public LocatorNode(string basePath, XElement node)
            : this(new XlinkNode(basePath, node))
        {
        }

        public LocatorNode(XlinkNode node)
        {
            Locator = node;
            Parents = new List<LocatorNode>();
            Children = new List<LocatorNode>();
        }
    }
}
