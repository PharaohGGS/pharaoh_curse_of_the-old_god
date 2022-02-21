using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;

namespace Pharaoh.AI.Actions
{
    public class CheckHoldingWeapon : ActionNode
    {
        private WeaponHolder _holder = null;

        protected override void OnStart()
        {
            if (_holder) return;

            var holder = agent.TryGetComponent(out WeaponHolder h)
                ? h : agent.GetComponentInChildren<WeaponHolder>();

            if (!holder?.weapon)
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

            if (_holder.weapon.transform.parent)
            {
                state = NodeState.Success;
            }
            else
            {
                state = NodeState.Failure;
                if (_holder.weapon.isThrown && _holder.weapon.isOnGround)
                {
                    blackboard.SetData("target", _holder.weapon.transform);
                }
            }

            return state;
        }
    }
}