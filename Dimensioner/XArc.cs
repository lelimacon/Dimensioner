using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner
{
    public class XArc : XlinkNode
    {
        public double? Order { get; set; }

        public XArc(string basePath, XElement node)
            : base(basePath, node)
        {
            string orderStr = node.Attr("order");
            Order = orderStr == null ? null as double? : double.Parse(orderStr);
        }
    }
}
