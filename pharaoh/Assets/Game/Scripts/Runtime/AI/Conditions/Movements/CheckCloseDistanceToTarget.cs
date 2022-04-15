using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckCloseDistanceToTarget : ActionNode
    {
        private AiMovement _aiMovement = null;
        [SerializeField] private bool ignoreHeight = true;

        protected override void OnStart()
        {
            if (_aiMovement || agent.TryGetComponent(out _aiMovement)) return;
            LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
        }

        protected override NodeState OnUpdate()
        {
            if (!_aiMovement || !blackboard.TryGetData("target", out Transform target)) return NodeState.Failure;
            
            var distance = ignoreHeight
                ? Mathf.Abs(agent.transform.position.x - target.position.x)
                : Vector2.Distance(agent.transform.position, target.position);

            return distance <= _aiMovement.closeDistance ? NodeState.Success : NodeState.Failure;

        }
    }
}