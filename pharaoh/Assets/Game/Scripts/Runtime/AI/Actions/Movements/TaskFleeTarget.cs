using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskFleeTarget : ActionNode
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
            if (_movement && blackboard.TryGetData("target", out Transform t))
            {
                var tr = agent.transform;
                var fleePosition = tr.position - (t.position - tr.position);
                if (Vector3.Distance(tr.position, t.position) <= _movement.fleeDistance)
                {
                    tr.position = Vector3.MoveTowards(
                        tr.position, fleePosition,
                        _movement.moveSpeed * Time.deltaTime);
                }
            }

            state = NodeState.Running;
            return state;
        }
    }
}