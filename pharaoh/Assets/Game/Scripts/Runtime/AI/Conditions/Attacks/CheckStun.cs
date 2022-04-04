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
            if (_aiMovement) return;

            if (agent.TryGetComponent(out _aiMovement)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't be stunned", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_aiMovement) return NodeState.Failure;
            return _aiMovement.isStunned ? NodeState.Success : NodeState.Failure;
        }
    }
}