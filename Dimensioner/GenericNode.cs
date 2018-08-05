using System.Collections.Generic;
using Dimensioner.Components.Arcroles;

namespace Dimensioner
{
    public class GenericArc
    {
        //public int? Order { get; set; }
        public Arcrole Arcrole { get; set; }
    }

    public class GenericNode<TNode, TArc, TValue>
        where TNode : GenericNode<TNode, TArc, TValue>
        where TArc : GenericArc
    {
        public List<KeyValuePair<TArc, TNode>> Children { get; set; }
        public TValue Value { get; set; }
    }

    public class GenericNode<TNode, TValue>
        where TNode : GenericNode<TNode, TValue>
    {
        public List<TNode> Children { get; set; }
        public TValue Value { get; set; }
    }
}
