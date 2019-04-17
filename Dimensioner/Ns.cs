using System.Collections.Generic;

namespace Dimensioner
{
    /// <summary>
    ///     Class to store all of the XML and XBRL namespaces.
    ///     TODO: Change types to XNamespace.
    /// </summary>
    public static class Ns
    {
        // XML
        public const string Xs = "http://www.w3.org/2001/XMLSchema";

        public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string Xlink = "http://www.w3.org/1999/xlink";
        public const string Xml = "http://www.w3.org/XML/1998/namespace";

        // XBRL
        public const string Xbrli = "http://www.xbrl.org/2003/instance";

        public const string Xbrldt = "http://xbrl.org/2005/xbrldt";
        public const string Xl = "http://www.xbrl.org/2003/XLink";
        public const string Link = "http://www.xbrl.org/2003/linkbase";
        public const string Model = "http://www.eurofiling.info/xbrl/ext/model";
        public const string Find = "http://www.eurofiling.info/xbrl/ext/filing-indicators";
        public const string Gen = "http://xbrl.org/2008/generic";
        public const string Label = "http://xbrl.org/2008/label";

        // Linkbase roles
        public const string XbrlCalculationLinkbaseReferenceRole =
            "http://www.xbrl.org/2003/role/calculationLinkbaseRef";

        public const string XbrlDefinitionLinkbaseReferenceRole =
            "http://www.xbrl.org/2003/role/definitionLinkbaseRef";

        public const string XbrlLabelLinkbaseReferenceRole = "http://www.xbrl.org/2003/role/labelLinkbaseRef";

        public const string XbrlPresentationLinkbaseReferenceRole =
            "http://www.xbrl.org/2003/role/presentationLinkbaseRef";

        public const string XbrlReferenceLinkbaseReferenceRole = "http://www.xbrl.org/2003/role/referenceLinkbaseRef";

        // Role URIs
        public const string XbrlLinkRole = "http://www.xbrl.org/2003/role/link";

        public const string XbrlLabelRole = "http://www.xbrl.org/2003/role/label";
        public const string XbrlTerseLabelRole = "http://www.xbrl.org/2003/role/terseLabel";
        public const string XbrlVerboseLabelRole = "http://www.xbrl.org/2003/role/verboseLabel";
        public const string XbrlPositiveLabelRole = "http://www.xbrl.org/2003/role/positiveLabel";
        public const string XbrlPositiveTerseLabelRole = "http://www.xbrl.org/2003/role/positiveTerseLabel";
        public const string XbrlPositiveVerboseLabelRole = "http://www.xbrl.org/2003/role/positiveVerboseLabel";
        public const string XbrlNegativeLabelRole = "http://www.xbrl.org/2003/role/negativeLabel";
        public const string XbrlNegativeTerseLabelRole = "http://www.xbrl.org/2003/role/negativeTerseLabel";
        public const string XbrlNegativeVerboseLabelRole = "http://www.xbrl.org/2003/role/negativeVerboseLabel";
        public const string XbrlZeroLabelRole = "http://www.xbrl.org/2003/role/zeroLabel";
        public const string XbrlZeroTerseLabelRole = "http://www.xbrl.org/2003/role/zeroTerseLabel";
        public const string XbrlZeroVerboseLabelRole = "http://www.xbrl.org/2003/role/zeroVerboseLabel";
        public const string XbrlTotalLabelRole = "http://www.xbrl.org/2003/role/totalLabel";
        public const string XbrlPeriodStartLabelRole = "http://www.xbrl.org/2003/role/periodStartLabel";
        public const string XbrlPeriodEndLabelRole = "http://www.xbrl.org/2003/role/periodEndLabel";
        public const string XbrlDocumentationRole = "http://www.xbrl.org/2003/role/documentation";
        public const string XbrlDocumentationGuidanceRole = "http://www.xbrl.org/2003/role/definitionGuidance";
        public const string XbrlDisclosureGuidanceRole = "http://www.xbrl.org/2003/role/disclosureGuidance";
        public const string XbrlPresentationGuidanceRole = "http://www.xbrl.org/2003/role/presentationGuidance";
        public const string XbrlMeasurementGuidanceRole = "http://www.xbrl.org/2003/role/measurementGuidance";
        public const string XbrlCommentaryGuidanceRole = "http://www.xbrl.org/2003/role/commentaryGuidance";
        public const string XbrlExampleGuidanceRole = "http://www.xbrl.org/2003/role/exampleGuidance";
        public const string XbrlRcCodeRole = "http://www.xbrl.org/2003/role/rc-code";

        // Table
        public static List<string> XbrlTableLinkbase =
            new List<string> {"http://xbrl.org/2014/table", "http://xbrl.org/PWD/2013-05-17/table"};

        public const string XbrlFormula = "http://xbrl.org/2008/formula";
        public const string XbrlFilterDimension = "http://xbrl.org/2008/filter/dimension";
    }
}
