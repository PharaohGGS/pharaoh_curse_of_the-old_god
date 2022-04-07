using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskFleeTarget : ActionNode
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
            if (_aiMovement && blackboard.TryGetData("target", out Transform t))
            {
                var agentPosition = agent.transform.position;
                var fleePosition = agentPosition - (t.position - agentPosition);
                agent.transform.position = Vector2.MoveTowards(
                    agentPosition, fleePosition,
                    _aiMovement.moveSpeed * Time.deltaTime);
            }

            state = NodeState.Running;
            return state;
        }
    }
}