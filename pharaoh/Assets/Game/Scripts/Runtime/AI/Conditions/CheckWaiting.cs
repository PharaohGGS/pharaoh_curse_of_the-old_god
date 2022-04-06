using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;

namespace Pharaoh.AI.Actions
{
    public class CheckWaiting : ActionNode
    {
        protected override NodeState OnUpdate()
        {
            bool isWaiting = blackboard.TryGetData("isWaiting", out bool value) && value;
            return isWaiting ? NodeState.Success : NodeState.Failure;
        }
    }
}