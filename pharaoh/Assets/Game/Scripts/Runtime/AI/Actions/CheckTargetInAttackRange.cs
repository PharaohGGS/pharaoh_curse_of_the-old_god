using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInAttackRange : ActionNode
    {
        [SerializeField] private WeaponHolder _holder = null;

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
            if (!blackboard.TryGetData("target", out Transform t) || 
                !_holder || !_holder.weapon || _holder.weapon.transform.parent == null)
            {
                state = NodeState.Failure;
                return state;
            }

            if (Vector3.Distance(agent.transform.position, t.position) <= _holder.weapon.data.attackRange)
            {
                blackboard.SetData("waitTime", _holder.weapon.data.attackRate);
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Failure;
            return state;
        }
    }
}