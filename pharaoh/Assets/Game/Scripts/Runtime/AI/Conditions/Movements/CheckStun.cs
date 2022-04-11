using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;

namespace Pharaoh.AI.Actions
{
    public class CheckStun : ActionNode
    {
        private AiMovement _aiMovement = null;

        protected override void OnStart()
        {
            if (_aiMovement || agent.TryGetComponent(out _aiMovement)) return;
            LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
        }

        protected override NodeState OnUpdate()
        {
            return _aiMovement && _aiMovement.isStunned ? NodeState.Success : NodeState.Failure;
        }
    }
}