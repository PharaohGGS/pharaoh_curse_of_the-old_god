using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckCloseDistanceToTarget : ActionNode
    {
        private AiMovement _aiMovement = null;

        protected override void OnStart()
        {
            if (_aiMovement || agent.TryGetComponent(out _aiMovement)) return;
            LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
        }

        protected override NodeState OnUpdate()
        {
            if (!_aiMovement) return NodeState.Failure;

            var target = blackboard.GetData<Transform>("target").position;
            var position = agent.transform.position;
            var closeDistance = _aiMovement.closeDistance;

            return Vector2.Distance(position, target) <= closeDistance 
                ? NodeState.Success : NodeState.Failure;
        }
    }
}