using Dimensioner.Components.Elements;

namespace Dimensioner.Components.Calculations
{
    public class CalculationNode : GenericNode<CalculationNode, CalculationArc, XbrlElement>
    {
    }

    public class CalculationArc : GenericArc
    {
        public int Weight { get; set; }
    }

    /*
    public class CalculationNode
    {
        public List<CalculationArc> Children { get; set; }
        public XbrlElement Element { get; set; }
        public int Weight { get; set; }
    }
    public class CalculationArc
    {
        public CalculationNode Node { get; set; }
        public int? Order { get; set; }
        public int Weight { get; set; }
        public Arcrole Arcrole { get; set; }
    }
    */
}
