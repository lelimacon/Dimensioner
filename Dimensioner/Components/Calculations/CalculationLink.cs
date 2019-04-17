namespace Dimensioner.Components.Calculations
{
    public class CalculationLink : Link<CalculationArc>
    {
        public CalculationLink(XbrlSchema schema, string id)
            : base(schema, id)
        {
        }
    }
}
