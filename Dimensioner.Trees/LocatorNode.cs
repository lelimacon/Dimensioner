using Dimensioner.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Dimensioner.Trees
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

        public static IEnumerable<LocatorNode> ToLocatorGraph(
            string basePath, XElement node, bool tree)
        {
            var locs = node.Children(Ns.Link, "loc")
                .Select(n => new LocatorNode(basePath, n))
                .ToList();
            var arcs = node.Elements()
                .Where(n => n.Name.Namespace == Ns.Link
                            && n.Attr(Ns.Xlink, "type") == "arc")
                .Select(n => new XArc(basePath, n))
                .OrderBy(a => a.Order)
                .ToList();

            // Index the access.
            var fromArcs = arcs.ToLookup(a => a.From);
            var toArcs = arcs.ToLookup(a => a.To);

            // Fetch child and parent locators.
            foreach (LocatorNode loc in locs)
            {
                var childArcs = fromArcs[loc.Locator.Label].ToList();
                var childLocs = locs.Where(l => childArcs.Any(a => a.To == l.Locator.Label));
                loc.Children = childLocs.ToList();

                var parentArcs = toArcs[loc.Locator.Label].ToList();
                var parentLocs = locs.Where(l => parentArcs.Any(a => a.From == l.Locator.Label));
                loc.Parents = parentLocs.ToList();

                if (tree && parentArcs.Count > 1)
                    throw new Exception("Multiple parents were not expected.");

                loc.ChildArcs = childArcs;
            }

            // Return roots (locators with no parents).
            return locs.Where(l => l != null && !l.Parents.Any());
        }
    }
}
