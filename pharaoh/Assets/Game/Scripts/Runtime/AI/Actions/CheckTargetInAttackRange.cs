using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInAttackRange : ActionNode
    {
        [SerializeField] private DamagerHolder _holder = null;

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
            if (!blackboard.TryGetData("target", out Transform t) || 
                !_holder || !_holder.damager || _holder.damager.transform.parent == null)
            {
                state = NodeState.Failure;
                return state;
            }

            if (Vector3.Distance(agent.transform.position, t.position) <= _holder.damager.data.attackRange)
            {
                blackboard.SetData("waitTime", _holder.damager.data.attackRate);
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Failure;
            return state;
        }
    }
}