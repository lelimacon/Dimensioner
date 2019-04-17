using System.Xml.Linq;

namespace Dimensioner.Tables.Formula
{
    /// <summary>
    ///     An encapsulation of the XBRL "concept" element
    ///     as defined in the formula schema at http://www.xbrl.org/2008/formula.xsd.
    ///     Specifications: https://specifications.xbrl.org/work-product-index-formula-formula-1.0.html
    /// </summary>
    public class Concept
    {
        public XName QName { get; set; }
    }
}
