namespace Dimensioner.Components.Tables
{
    /// <summary>
    ///     An encapsulation of the XBRL "openDefinitionNode" element
    ///     as defined in the table linkbase schema at https://www.xbrl.org/2014/table.xsd.
    ///     Specifications:
    ///     http://www.xbrl.org/Specification/table-linkbase/REC-2014-03-18/table-linkbase-REC-2014-03-18+corrected-errata-2016-03-09.html
    /// </summary>
    public abstract class OpenDefinitionNode : DefinitionNode
    {
        internal OpenDefinitionNode(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
