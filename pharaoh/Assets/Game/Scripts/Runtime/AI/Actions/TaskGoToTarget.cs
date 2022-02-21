using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGoToTarget : ActionNode
    {
        private Pawn _pawn = null;

        protected override void OnStart()
        {
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (_pawn && blackboard.TryGetData("target", out Transform t))
            {
                var tr = agent.transform;
                if (Vector3.Distance(tr.position, t.position) > 0.01f)
                {
                    tr.position = Vector3.MoveTowards(
                        tr.position, t.position,
                        _pawn.movement.moveSpeed * Time.deltaTime);
                    tr.LookAt(t.position);
                }
            }

            state = NodeState.Running;
            return state;
        }
    }
}