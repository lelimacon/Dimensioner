using System.Linq;
using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Tables.Formula
{
    /// <summary>
    ///     An encapsulation of the XBRL "member" element
    ///     as defined in the formula schema at http://www.xbrl.org/2008/formula.xsd.
    ///     Specifications: https://specifications.xbrl.org/work-product-index-formula-formula-1.0.html
    /// </summary>
    public class Member
    {
        public XName QName { get; }

        public Member(XElement node)
        {
            QName = node.ToXName(node.Children(Ns.XbrlFormula, "qname").Single().Value);
        }
    }
}
