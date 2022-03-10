using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckHoldingGear : ActionNode
    {
        [SerializeField] private GearType gearType;

        private AttackComponent _attack;

        protected override void OnStart()
        {
            if (_attack) return;

            if (agent.TryGetComponent(out _attack)) return;

            LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            return !_attack || gearType == GearType.Null || !_attack.TryGetHolder(gearType, out var holder) || !holder.gear.transform.parent
                ? NodeState.Failure : NodeState.Success;
        }

        protected override void OnStop()
        {
            if (state != NodeState.Failure) return;
            if (!_attack || gearType == GearType.Null || !_attack.TryGetHolder(gearType, out var holder)) return;

            if (holder.gear.isThrown && holder.gear.isGrounded)
            {
                blackboard.SetData("target", holder.gear.transform);
            }
        }
    }
}