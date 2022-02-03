using Pharaoh.Tools.BehaviourTree.ScriptableObjects;

namespace Pharaoh.AI.Decorators
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
