using System.Collections.Generic;

namespace Dimensioner
{
    /// <summary>
    ///     Class to store all of the XML and XBRL namespaces.
    /// </summary>
    internal static class Ns
    {
        // XML
        public const string Xs = "http://www.w3.org/2001/XMLSchema";

        public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string Xlink = "http://www.w3.org/1999/xlink";
        internal const string Xml = "http://www.w3.org/XML/1998/namespace";

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
        internal const string XbrlCalculationLinkbaseReferenceRole =
            "http://www.xbrl.org/2003/role/calculationLinkbaseRef";

        internal const string XbrlDefinitionLinkbaseReferenceRole =
            "http://www.xbrl.org/2003/role/definitionLinkbaseRef";

        internal const string XbrlLabelLinkbaseReferenceRole = "http://www.xbrl.org/2003/role/labelLinkbaseRef";

        internal const string XbrlPresentationLinkbaseReferenceRole =
            "http://www.xbrl.org/2003/role/presentationLinkbaseRef";

        internal const string XbrlReferenceLinkbaseReferenceRole = "http://www.xbrl.org/2003/role/referenceLinkbaseRef";

        // Role URIs
        internal const string XbrlLinkRole = "http://www.xbrl.org/2003/role/link";

        internal const string XbrlLabelRole = "http://www.xbrl.org/2003/role/label";
        internal const string XbrlTerseLabelRole = "http://www.xbrl.org/2003/role/terseLabel";
        internal const string XbrlVerboseLabelRole = "http://www.xbrl.org/2003/role/verboseLabel";
        internal const string XbrlPositiveLabelRole = "http://www.xbrl.org/2003/role/positiveLabel";
        internal const string XbrlPositiveTerseLabelRole = "http://www.xbrl.org/2003/role/positiveTerseLabel";
        internal const string XbrlPositiveVerboseLabelRole = "http://www.xbrl.org/2003/role/positiveVerboseLabel";
        internal const string XbrlNegativeLabelRole = "http://www.xbrl.org/2003/role/negativeLabel";
        internal const string XbrlNegativeTerseLabelRole = "http://www.xbrl.org/2003/role/negativeTerseLabel";
        internal const string XbrlNegativeVerboseLabelRole = "http://www.xbrl.org/2003/role/negativeVerboseLabel";
        internal const string XbrlZeroLabelRole = "http://www.xbrl.org/2003/role/zeroLabel";
        internal const string XbrlZeroTerseLabelRole = "http://www.xbrl.org/2003/role/zeroTerseLabel";
        internal const string XbrlZeroVerboseLabelRole = "http://www.xbrl.org/2003/role/zeroVerboseLabel";
        internal const string XbrlTotalLabelRole = "http://www.xbrl.org/2003/role/totalLabel";
        internal const string XbrlPeriodStartLabelRole = "http://www.xbrl.org/2003/role/periodStartLabel";
        internal const string XbrlPeriodEndLabelRole = "http://www.xbrl.org/2003/role/periodEndLabel";
        internal const string XbrlDocumentationRole = "http://www.xbrl.org/2003/role/documentation";
        internal const string XbrlDocumentationGuidanceRole = "http://www.xbrl.org/2003/role/definitionGuidance";
        internal const string XbrlDisclosureGuidanceRole = "http://www.xbrl.org/2003/role/disclosureGuidance";
        internal const string XbrlPresentationGuidanceRole = "http://www.xbrl.org/2003/role/presentationGuidance";
        internal const string XbrlMeasurementGuidanceRole = "http://www.xbrl.org/2003/role/measurementGuidance";
        internal const string XbrlCommentaryGuidanceRole = "http://www.xbrl.org/2003/role/commentaryGuidance";
        internal const string XbrlExampleGuidanceRole = "http://www.xbrl.org/2003/role/exampleGuidance";
        internal const string XbrlRcCodeRole = "http://www.xbrl.org/2003/role/rc-code";

        // Table
        internal static List<string> XbrlTableLinkbase =
            new List<string> {"http://xbrl.org/2014/table", "http://xbrl.org/PWD/2013-05-17/table"};

        internal const string XbrlFormula = "http://xbrl.org/2008/formula";
        internal const string XbrlFilterDimension = "http://xbrl.org/2008/filter/dimension";
    }
}
