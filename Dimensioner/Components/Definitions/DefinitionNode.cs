using System.Collections.Generic;
using System.Linq;
using Dimensioner.Components.Arcroles;
using Dimensioner.Components.Elements;

namespace Dimensioner.Components.Definitions
{
    public class DefinitionNode
    {
        public List<DefinitionNode> Children { get; set; }
        public XbrlElement Element { get; set; }

        public Arcrole Arcrole { get; set; }

        //public string Label { get; set; }
        public double? Order { get; set; }

        public IEnumerable<DefinitionNode> Flatten =>
            new List<DefinitionNode> {this}
                .Concat(Children.SelectMany(n => n.Flatten));
    }
}
