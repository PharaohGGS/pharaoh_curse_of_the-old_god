using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckTargetInAttackRange : ActionNode
    {
        [SerializeField] private GearData gearData;

        private AttackComponent _attack;

        protected override void OnStart()
        {
            if (_attack) return;

            if (agent.TryGetComponent(out _attack)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_attack || !gearData || !_attack.TryGetHolder(gearData, out var holder) || 
                !blackboard.TryGetData("target", out Transform t))
            {
                return NodeState.Failure;
            }

            var distance = Vector2.Distance(holder.gear.transform.position, t.position);
            return distance > gearData.range ? NodeState.Failure : NodeState.Success;
        }
    }
}