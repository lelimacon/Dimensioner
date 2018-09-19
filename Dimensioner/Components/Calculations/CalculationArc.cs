using Dimensioner.Components.Elements;

namespace Dimensioner.Components.Calculations
{
    public class CalculationArc : GenericArc
    {
        public int Weight { get; set; }
        public XbrlElement From { get; set; }
        public XbrlElement To { get; set; }
    }
}
