using Dimensioner.Components.Labels;

namespace Dimensioner
{
    public interface ILabelized
    {
        XbrlLabels Labels { get; set; }
    }
}
