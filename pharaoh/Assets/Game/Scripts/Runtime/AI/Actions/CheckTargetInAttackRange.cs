using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInAttackRange : ActionNode
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

            if (!blackboard.TryGetData("target", out Transform t) || !_attack)
            {
                state = NodeState.Failure;
                return state;
            }

            var holder = _attack.dataHolders[data];

            if (!holder || !holder.damager || holder.damager.transform.parent == null)
            {
                state = NodeState.Failure;
                return state;
            }

            if (Vector3.Distance(agent.transform.position, t.position) <= holder.damager.data.attackRange)
            {
                blackboard.SetData("waitTime", holder.damager.data.attackRate);
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Failure;
            return state;
        }
    }
}