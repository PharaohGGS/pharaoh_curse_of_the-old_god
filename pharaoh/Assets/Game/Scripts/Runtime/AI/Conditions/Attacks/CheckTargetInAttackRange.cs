using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public abstract class CheckTargetInAttackRange<T> : ActionNode where T : DamagerData
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
            state = NodeState.Failure;
            if (!_attack || !blackboard.TryGetData("target", out Transform t))
            {
                return state;
            }

            if (!_attack.dataHolders.TryGetValue(data, out var holder))
            {
                return state;
            }

            if (!holder || !holder.damager || holder.damager.transform.parent == null)
            {
                return state;
            }

            if (Vector3.Distance(agent.transform.position, t.position) <= holder.damager.data.attackRange)
            {
                blackboard.SetData("waitTime", holder.damager.data.attackRate);
                state = NodeState.Success;
            }
            
            return state;
        }
    }
}