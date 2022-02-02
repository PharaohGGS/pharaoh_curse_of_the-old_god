using System.Collections.Generic;

namespace Pharaoh.Tools.BehaviourTree
{
    public class Selector<T> : Selector where T : Tree
    {
        protected T tree;

        public Selector(T tree) : base() { }
        public Selector(T tree, List<Node> children) : base(children) { }
    }

    /// <summary>
    /// Act like '||' logic gate
    /// </summary>
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (var node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.FAILURE:
                        continue;
                    default:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}