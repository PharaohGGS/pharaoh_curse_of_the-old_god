using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckCloseDistanceToTarget : ActionNode
    {
        private MovementComponent _movement = null;

        protected override void OnStart()
        {
            if (_movement == null && !agent.TryGetComponent(out _movement))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (!_movement || !blackboard.TryGetData("target", out Transform t)) return NodeState.Failure;
            
            return Vector2.Distance(agent.transform.position, t.position) <= _movement.closeDistance 
                ? NodeState.Success : NodeState.Failure;
        }
    }
}