using BehaviourTree.Tools;
using UnityEngine;

namespace BehaviourTree.Runtime.Composites
{
    public class SelectorNode : CompositeNode
    {
        private int _current;

        protected override void OnStart()
        {
            _current = 0;
        }

        protected override NodeState OnUpdate()
        {
            var child = children[_current];

            switch (child.Evaluate())
            {
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Success:
                    return NodeState.Success;
                case NodeState.Failure:
                    _current++;
                    return NodeState.Failure;
                default:
                    _current++;
                    break;
            }

            return _current == children.Count ? NodeState.Success : NodeState.Running;
        }
    }

}
