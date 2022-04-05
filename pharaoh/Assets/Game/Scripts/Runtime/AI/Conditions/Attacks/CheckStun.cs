using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;

namespace Pharaoh.AI.Actions
{
    public class CheckStun : ActionNode
    {
        private AiMovement _aiMovement; 
        
        protected override void OnStart()
        {
            if (_aiMovement || agent.TryGetComponent(out _aiMovement)) return;
            LogHandler.SendMessage($"[{agent.name}] Can't be stunned", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            return _aiMovement?.isStunned == true ? NodeState.Success : NodeState.Failure;
        }
    }
}