using System.Collections.Generic;
using System.Xml.Linq;

namespace Dimensioner
{
    public class GenericLinkNode<T> : XlinkNode
    {
        public List<GenericLinkNode<T>> Parents { get; set; }
        public List<GenericLinkNode<T>> Children { get; set; }
        public T Item { get; set; }

        public GenericLinkNode(string basePath, XElement node)
            : base(basePath, node)
        {
        }
    }
}
