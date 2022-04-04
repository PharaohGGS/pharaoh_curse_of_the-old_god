using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckFleeDistanceToTarget : ActionNode
    {
        private AiMovement _aiMovement = null;

        protected override void OnStart()
        {
            if (_aiMovement == null && !agent.TryGetComponent(out _aiMovement))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (!_aiMovement || !blackboard.TryGetData("target", out Transform t)) return NodeState.Failure;
            
            return Vector2.Distance(agent.transform.position, t.position) <= _aiMovement.fleeDistance 
                ? NodeState.Failure : NodeState.Success;
        }
    }
}