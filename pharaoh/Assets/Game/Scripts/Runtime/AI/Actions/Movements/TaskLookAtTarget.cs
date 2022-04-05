using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskLookAtTarget : ActionNode
    {
        private AiMovement _aiMovement;

        protected override void OnStart()
        {
            if (_aiMovement || agent.TryGetComponent(out _aiMovement)) return;
            LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
        }

        protected override NodeState OnUpdate()
        {
            if (!_aiMovement) return NodeState.Failure;
            _aiMovement.LookAt(blackboard.GetData<Transform>("target").position);
            return NodeState.Running;
        }
    }
}