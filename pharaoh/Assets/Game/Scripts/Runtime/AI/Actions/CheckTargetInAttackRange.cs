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
            if (!blackboard.TryGetData("target", out Transform t) || 
                !_pawn || !_pawn.holder.weapon || _pawn.holder.weapon.transform.parent == null)
            {
                state = NodeState.Failure;
                return state;
            }

            if (Vector3.Distance(agent.transform.position, t.position) <= _pawn.holder.weapon.data.attackRange)
            {
                blackboard.SetData("waitTime", _pawn.holder.weapon.data.attackRate);
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Failure;
            return state;
        }
    }
}