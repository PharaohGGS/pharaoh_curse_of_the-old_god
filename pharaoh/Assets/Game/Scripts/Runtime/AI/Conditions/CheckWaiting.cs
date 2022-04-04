using BehaviourTree.Tools;

namespace Pharaoh.AI.Actions
{
    public class CheckWaiting : ActionNode
    {
        protected override NodeState OnUpdate()
        {
            state = !blackboard.TryGetData("isWaiting", out bool value) || !value
                ? NodeState.Failure : NodeState.Success;
            return state;
        }
    }
}