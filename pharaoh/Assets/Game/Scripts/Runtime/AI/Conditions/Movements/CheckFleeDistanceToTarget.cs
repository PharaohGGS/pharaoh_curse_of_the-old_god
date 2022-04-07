using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckFleeDistanceToTarget : ActionNode
    {
        private AiMovement _aiMovement = null;
        [SerializeField] private bool ignoreHeight = true;

        protected override void OnStart()
        {
            if (_aiMovement == null && !agent.TryGetComponent(out _aiMovement))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (!_aiMovement) return NodeState.Failure;
            
            var target = blackboard.GetData<Transform>("target").position;
            var position = agent.transform.position;
            var fleeDistance = _aiMovement.fleeDistance;

            var distance = ignoreHeight
                ? Mathf.Abs(position.x - target.x)
                : Vector2.Distance(position, target);

            return distance > fleeDistance ? NodeState.Success : NodeState.Failure;
        }
    }
}