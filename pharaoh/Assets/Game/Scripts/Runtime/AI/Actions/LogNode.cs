
using BehaviourTree.Tools;
using Pharaoh.Tools.Debug;

namespace Pharaoh.AI.Actions
{
    public class LogNode : ActionNode
    {
        public string message;
        public MessageType type;

        protected override void OnStart()
        {
            Blackboard.SetData("message", message);
        }

        protected override NodeState OnUpdate()
        {
            LogHandler.SendMessage($"{message}", type);
            return NodeState.Success;
        }
    }
}