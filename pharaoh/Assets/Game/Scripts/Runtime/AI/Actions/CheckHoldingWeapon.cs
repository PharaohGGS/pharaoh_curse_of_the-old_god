using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;

namespace Pharaoh.AI.Actions
{
    public class CheckHoldingWeapon : ActionNode
    {
        public DamagerData data;
        private AttackComponent _attack;

        protected override void OnStart()
        {
            if (_attack) return;

            if (agent.TryGetComponent(out _attack)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_attack || !_attack.dataHolders[data])
            {
                state = NodeState.Failure;
                return state;
            }

            var holder = _attack.dataHolders[data];

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