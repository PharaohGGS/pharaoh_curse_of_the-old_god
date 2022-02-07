
using BehaviourTree.Tools;

namespace Pharaoh.AI.Actions
{
    public class LogNode : ActionNode
    {
        public string message;
        //public MessageType type;

        protected override NodeState OnUpdate()
        {
            //LogHandler.SendMessage($"{message}", type);
            return NodeState.Success;
        }
    }
}