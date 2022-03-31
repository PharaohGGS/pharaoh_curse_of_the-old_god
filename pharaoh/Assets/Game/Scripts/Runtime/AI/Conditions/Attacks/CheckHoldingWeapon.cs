using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class CheckHoldingWeapon : ActionNode
    {
        private FightComponent _fight;

        protected override void OnStart()
        {
            if (_fight) return;

            if (agent.TryGetComponent(out _fight) == true) return;

            LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            return !_fight || !_fight.activeWeapon || !_fight.activeWeapon.transform.parent
                ? NodeState.Failure : NodeState.Success;
        }

        protected override void OnStop()
        {
            if (state != NodeState.Failure || !_fight) return;
            var weapon = _fight.activeWeapon;
            if (!weapon) return;

            if (weapon.isThrown && weapon.isGrounded)
            {
                blackboard.SetData("target", weapon.transform);
            }
        }
    }
}