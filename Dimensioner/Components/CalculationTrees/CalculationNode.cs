using Dimensioner.Components.Elements;

namespace Dimensioner.Components.CalculationTrees
{
    public class CalculationNode : GenericNode<CalculationNode, CalculationArc, XbrlElement>
    {
    }

    public class CalculationArc : GenericArc
    {
        public int Weight { get; set; }
    }
}
