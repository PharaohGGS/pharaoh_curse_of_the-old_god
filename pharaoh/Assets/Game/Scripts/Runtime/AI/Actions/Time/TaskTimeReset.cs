using BehaviourTree.Tools;

namespace Pharaoh.AI.Actions
{
    public class TaskTimeReset : ActionNode
    {
        protected override NodeState OnUpdate()
        {
            blackboard.ClearData("isWaiting");
            blackboard.ClearData("timeSince");
            blackboard.ClearData("waitTime");
            return NodeState.Success;
        }
    }
}