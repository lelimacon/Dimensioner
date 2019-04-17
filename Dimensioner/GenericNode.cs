using System.Collections.Generic;

namespace Dimensioner
{
    public class GenericNode<TNode, TArc, TValue>
        where TNode : GenericNode<TNode, TArc, TValue>
        where TArc : Arc
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
