using System;
using System.Collections.Generic;

namespace Pharaoh.Tools.BehaviourTree
{
    public class Sequence<T> : Sequence where T : Tree
    {
        protected T tree;

        public Sequence(T tree) : base() { }
        public Sequence(T tree, List<Node> children) : base(children) { }
    }

    /// <summary>
    /// Act like '&&' logic gate
    /// </summary>
    public class Sequence : Node
    {
        public Sequence() : base() {}
        public Sequence(List<Node> children) : base(children) {}

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (var node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}