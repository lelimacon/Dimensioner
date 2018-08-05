using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Dimensioner.Utils;

namespace Dimensioner.Components.Labels
{
    /// <summary>
    ///     An encapsulation of the XBRL element "label" as defined in the http://www.xbrl.org/2003/linkbase namespace.
    /// </summary>
    public class Label
    {
        /// <summary>
        ///     A list of possible roles for a label.
        /// </summary>
        /// <remarks>
        ///     See Table 8 in section 5.2.2.2.2 in the XBRL Specification for more information about label roles.
        /// </remarks>
        public enum RoleEnum
        {
            /// <summary>
            ///     A unkown label.
            /// </summary>
            Unknown,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/label.
            /// </summary>
            Standard,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/terseLabel.
            /// </summary>
            Short,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/verboseLabel.
            /// </summary>
            Verbose,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/positiveLabel.
            /// </summary>
            StandardPositiveValue,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/positiveTerseLabel.
            /// </summary>
            ShortPositiveValue,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/positiveVerboseLabel.
            /// </summary>
            VerbosePositiveValue,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/negativeLabel.
            /// </summary>
            StandardNegativeValue,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/negativeTerseLabel.
            /// </summary>
            ShortNegativeValue,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/negativeVerboseLabel.
            /// </summary>
            VerboseNegativeValue,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/zeroLabel.
            /// </summary>
            StandardZeroValue,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/zeroTerseLabel.
            /// </summary>
            ShortZeroValue,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/zeroVerboseLabel.
            /// </summary>
            VerboseZeroValue,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/totalLabel.
            /// </summary>
            Total,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/periodStartLabel.
            /// </summary>
            PeriodStart,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/periodEndLabel.
            /// </summary>
            PeriodEnd,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/documentation.
            /// </summary>
            Documentation,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/definitionGuidance.
            /// </summary>
            DefinitionGuidance,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/disclosureGuidance.
            /// </summary>
            DisclosureGuidance,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/presentationGuidance.
            /// </summary>
            PresentationGuidance,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/measurementGuidance.
            /// </summary>
            MeasurementGuidance,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/commentaryGuidance.
            /// </summary>
            CommentaryGuidance,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/exampleGuidance.
            /// </summary>
            ExampleGuidance,

            /// <summary>
            ///     A label with a role URI of http://www.xbrl.org/2003/role/rc-code.
            /// </summary>
            RcCode
        }

        /// <summary>
        ///     The role of this label.
        /// </summary>
        public RoleEnum LabelRole { get; private set; }

        /// <summary>
        ///     The text of this label.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     The culture of this label.
        /// </summary>
        public CultureInfo Culture { get; }

        public Label(XElement node)
        {
            Text = node.Value;
            LabelRole = GetRole(node.Attr(Ns.Xlink, "role"));
            string languageStr = node.Attr(Ns.Xml, "lang");
            Culture = new CultureInfo(languageStr);
        }

        public Label(XlinkNode node)
        {
            Text = node.XNode.Value;
            LabelRole = GetRole(node.Role);
            string languageStr = node.XNode.Attr(Ns.Xml, "lang");
            Culture = new CultureInfo(languageStr);
        }

        /// <summary>
        ///     Conversion according to Table 8 in section 5.2.2.2.2 in the XBRL Spec.
        /// </summary>
        /// <param name="roleUri"></param>
        /// <returns></returns>
        private RoleEnum GetRole(string roleUri)
        {
            return RoleName(roleUri).Switch(new Dictionary<string, RoleEnum>
            {
                [RoleName(Ns.XbrlLabelRole)] = RoleEnum.Standard,
                [RoleName(Ns.XbrlTerseLabelRole)] = RoleEnum.Short,
                [RoleName(Ns.XbrlVerboseLabelRole)] = RoleEnum.Verbose,
                [RoleName(Ns.XbrlPositiveLabelRole)] = RoleEnum.StandardPositiveValue,
                [RoleName(Ns.XbrlPositiveTerseLabelRole)] = RoleEnum.ShortPositiveValue,
                [RoleName(Ns.XbrlPositiveVerboseLabelRole)] = RoleEnum.VerbosePositiveValue,
                [RoleName(Ns.XbrlNegativeLabelRole)] = RoleEnum.StandardNegativeValue,
                [RoleName(Ns.XbrlNegativeTerseLabelRole)] = RoleEnum.ShortNegativeValue,
                [RoleName(Ns.XbrlNegativeVerboseLabelRole)] = RoleEnum.VerboseNegativeValue,
                [RoleName(Ns.XbrlZeroLabelRole)] = RoleEnum.StandardZeroValue,
                [RoleName(Ns.XbrlZeroTerseLabelRole)] = RoleEnum.ShortZeroValue,
                [RoleName(Ns.XbrlZeroVerboseLabelRole)] = RoleEnum.VerboseZeroValue,
                [RoleName(Ns.XbrlTotalLabelRole)] = RoleEnum.Total,
                [RoleName(Ns.XbrlPeriodStartLabelRole)] = RoleEnum.PeriodStart,
                [RoleName(Ns.XbrlPeriodEndLabelRole)] = RoleEnum.PeriodEnd,
                [RoleName(Ns.XbrlDocumentationRole)] = RoleEnum.Documentation,
                [RoleName(Ns.XbrlDocumentationGuidanceRole)] = RoleEnum.DefinitionGuidance,
                [RoleName(Ns.XbrlDisclosureGuidanceRole)] = RoleEnum.DisclosureGuidance,
                [RoleName(Ns.XbrlPresentationGuidanceRole)] = RoleEnum.PresentationGuidance,
                [RoleName(Ns.XbrlMeasurementGuidanceRole)] = RoleEnum.MeasurementGuidance,
                [RoleName(Ns.XbrlCommentaryGuidanceRole)] = RoleEnum.CommentaryGuidance,
                [RoleName(Ns.XbrlExampleGuidanceRole)] = RoleEnum.ExampleGuidance,
                [RoleName(Ns.XbrlRcCodeRole)] = RoleEnum.RcCode
            }, RoleEnum.Unknown);
        }

        private static string RoleName(string uri)
        {
            return uri.Substring(uri.LastIndexOf('/') + 1);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
