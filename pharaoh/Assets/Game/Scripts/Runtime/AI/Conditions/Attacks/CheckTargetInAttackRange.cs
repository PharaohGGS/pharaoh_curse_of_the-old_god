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
        [SerializeField] private float distance;

        private AttackComponent _attack;

        protected override void OnStart()
        {
            if (_attack) return;

            if (agent.TryGetComponent(out _attack)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            if (!_attack || !gearData || !_attack.dataGears.TryGetValue(gearData, out var gear) || 
                !blackboard.TryGetData("target", out Transform t))
            {
                return NodeState.Failure;
            }

            var range = gearData.range;
            if (gearData is MeleeGearData {throwable: true} meleeGearData)
            {
                range = meleeGearData.throwableRange;
            }

            distance = Vector2.Distance(gear.transform.position, t.position);
            return distance > range ? NodeState.Failure : NodeState.Success;
        }
    }
}