using System;
using BehaviourTree.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInAttackRange : ActionNode
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
            var t = blackboard.GetData("target") as Transform;

            if (!t || !_pawn || !_pawn.weapon)
            {
                state = NodeState.Failure;
                return state;
            }

            if (Vector3.Distance(agent.transform.position, t.position) <= _pawn.weapon.data.attackRange)
            {
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Failure;
            return state;
        }
    }
}