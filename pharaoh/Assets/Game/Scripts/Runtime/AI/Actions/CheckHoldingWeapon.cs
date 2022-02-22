using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;

namespace Pharaoh.AI.Actions
{
    public class CheckHoldingWeapon : ActionNode
    {
        private DamagerHolder _holder = null;

        protected override void OnStart()
        {
            if (_holder) return;

            var holder = agent.TryGetComponent(out DamagerHolder h)
                ? h : agent.GetComponentInChildren<DamagerHolder>();

            if (!holder?.damager)
            {
                LogHandler.SendMessage($"[{agent.name}] Can't attack enemies", MessageType.Warning);
                return;
            }

            _holder = holder;
        }

        protected override NodeState OnUpdate()
        {
            if (!_holder)
            {
                state = NodeState.Failure;
                return state;
            }

            if (_holder.damager.transform.parent)
            {
                state = NodeState.Success;
            }
            else
            {
                state = NodeState.Failure;
                if (_holder.damager is not Weapon weapon) return state;

                if (weapon.isThrown && weapon.isOnGround)
                {
                    blackboard.SetData("target", _holder.damager.transform);
                }
            }

            return state;
        }
    }
}