using Dimensioner.Components.Arcroles;
using Dimensioner.Components.Elements;

namespace Dimensioner
{
    public class Arc
    {
        public XbrlElement From { get; set; }
        public XbrlElement To { get; set; }
        public double? Order { get; set; }
        public Arcrole Arcrole { get; set; }
    }
}
