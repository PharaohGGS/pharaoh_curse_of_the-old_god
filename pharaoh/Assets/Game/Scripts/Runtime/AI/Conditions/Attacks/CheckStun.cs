using System.Runtime.InteropServices;
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
            if (!_aiMovement && !agent.TryGetComponent(out _aiMovement))
            {
                LogHandler.SendMessage($"[{agent.name}] Can't be stunned", MessageType.Warning);
                return;
            }

            if (!blackboard.TryGetData("isStunned", out bool isStunned) || !isStunned || !_aiMovement.isStunned) return;
            blackboard.ClearData("isStunned");
            _aiMovement.EndStun();
        }

        protected override NodeState OnUpdate()
        {
            return _aiMovement && _aiMovement.isStunned && _aiMovement.lastStunData
                ? NodeState.Success : NodeState.Failure;
        }
    }
}