
using BehaviourTree.Tools;

namespace BehaviourTree.Runtime.Decorators
{
    public class RepeatNode : DecoratorNode
    {
        protected override NodeState OnUpdate()
        {
            child.Evaluate();
            return NodeState.Running;
        }
    }
}
