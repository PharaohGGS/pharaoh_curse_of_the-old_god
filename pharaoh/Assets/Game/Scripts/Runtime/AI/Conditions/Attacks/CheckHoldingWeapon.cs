using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;

namespace Pharaoh.AI.Actions
{
    public abstract class CheckHoldingWeapon<T> : ActionNode where T : DamagerData
    {
        public T data;
        private AttackComponent _attack;

        protected override void OnStart()
        {
            if (_attack) return;

            if (agent.TryGetComponent(out _attack)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_attack)
            {
                state = NodeState.Failure;
                return state;
            }

            if (!_attack.dataHolders.TryGetValue(data, out DamagerHolder holder))
            {
                state = NodeState.Failure;
                return state;
            }

            if (holder.damager.transform.parent)
            {
                state = NodeState.Success;
            }
            else
            {
                state = NodeState.Failure;
                if (holder.damager is not Weapon weapon) return state;

                if (weapon.isThrown && weapon.isOnGround)
                {
                    blackboard.SetData("target", holder.damager.transform);
                }
            }

            return state;
        }
    }
}