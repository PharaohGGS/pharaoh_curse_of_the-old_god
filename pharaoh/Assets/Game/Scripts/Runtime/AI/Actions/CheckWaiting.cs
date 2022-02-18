using BehaviourTree.Tools;

namespace Pharaoh.AI.Actions
{
    public class CheckWaiting : ActionNode
    {
        protected override NodeState OnUpdate()
        {
            state = blackboard.GetData("isWaiting") is true 
                ? NodeState.Success : NodeState.Failure;
            return state;
        }
    }
}