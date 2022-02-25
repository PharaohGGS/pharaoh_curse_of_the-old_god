using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public abstract class CheckTargetInAttackRange<T> : ActionNode where T : DamagerData
    {
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
            if (!_attack || !_attack.TryGetHolder<T>(out var holder))
            {
                return state;
            }

            if (!blackboard.TryGetData("target", out Transform t) || 
                Vector3.Distance(agent.transform.position, t.position) > holder.data.range)
            {
                return state;
            }

            blackboard.SetData("waitTime", holder.data.rate);
            state = NodeState.Success;
            return state;
        }
    }
}