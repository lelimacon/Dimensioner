using Dimensioner.Components.Elements;

namespace Dimensioner.Trees.CalculationTrees
{
    public class CalculationNode : GenericNode<CalculationNode, CalculationArc, XbrlElement>
    {
    }

    public class CalculationArc : Arc
    {
        public int Weight { get; set; }
    }
}
