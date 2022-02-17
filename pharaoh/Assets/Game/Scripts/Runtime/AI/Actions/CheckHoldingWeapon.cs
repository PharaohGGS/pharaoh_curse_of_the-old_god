using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;

namespace Pharaoh.AI.Actions
{
    public class CheckHoldingWeapon : ActionNode
    {
        private EnemyPawn _pawn = null;
        private WeaponHolder _holder = null;

        protected override void OnStart()
        {
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
                return;
            }

            if (_holder) return;

            if (_holder == null && !_pawn.holder)
            {
                LogHandler.SendMessage($"Can't have a weapon !", MessageType.Error);
                return;
            }

            _holder = _pawn.holder;
        }

        protected override NodeState OnUpdate()
        {
            if (!_pawn || !_pawn.holder)
            {
                state = NodeState.Failure;
                return state;
            }

            if (_holder.weapon.transform.parent && !_holder.weapon.isThrown)
            {
                state = NodeState.Success;
            }
            else
            {
                state = NodeState.Failure;
                blackboard.SetData("target", _pawn.holder.weapon.transform);
            }

            return state;
        }
    }
}