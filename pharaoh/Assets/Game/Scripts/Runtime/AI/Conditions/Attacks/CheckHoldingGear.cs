using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckHoldingGear : ActionNode
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
            return !_attack || !gearData || !_attack.dataGears.TryGetValue(gearData, out var gear) || !gear.transform.parent
                ? NodeState.Failure : NodeState.Success;
        }

        protected override void OnStop()
        {
            if (state != NodeState.Failure) return;
            if (!_attack || !gearData || !_attack.dataGears.TryGetValue(gearData, out var gear)) return;

            if (gear.isThrown && gear.isGrounded)
            {
                blackboard.SetData("target", gear.transform);
            }
        }
    }
}