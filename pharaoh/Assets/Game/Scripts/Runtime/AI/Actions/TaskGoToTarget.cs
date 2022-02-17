using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGoToTarget : ActionNode
    {
        private EnemyPawn _pawn = null;

        protected override void OnStart()
        {
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
            }
        }

        protected override NodeState OnUpdate()
        {
            var target = blackboard.GetData("target") as Transform;

            if (blackboard.GetData("isWaiting") is true) return state;
            
            if (target != null && _pawn)
            {
                var tr = agent.transform;
                if (Vector3.Distance(tr.position, target.position) > 0.01f)
                {
                    tr.position = Vector3.MoveTowards(
                        tr.position, target.position,
                        _pawn.movement.moveSpeed * Time.deltaTime);
                    tr.LookAt(target.position);
                }
            }

            state = NodeState.Running;
            return state;
        }
    }
}