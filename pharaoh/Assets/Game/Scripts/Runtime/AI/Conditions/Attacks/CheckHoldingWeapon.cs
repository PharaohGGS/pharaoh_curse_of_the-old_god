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
        [SerializeField] private bool holding;

        protected override void OnStart()
        {
            if (_fight || agent.TryGetComponent(out _fight)) return;
            LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
        }

        protected override NodeState OnUpdate()
        {
            holding = _fight && _fight.activeWeapon && 
                           _fight.activeWeapon.isActiveAndEnabled &&
                           !_fight.activeWeapon.isThrown;
            return holding ? NodeState.Success : NodeState.Failure;
        }

        protected override void OnStop()
        {
            if (!_fight || !_fight.activeWeapon || state != NodeState.Failure) return;

            var weapon = _fight.activeWeapon;
            if (weapon.isThrown && weapon.isGrounded)
            {
                blackboard.SetData("target", weapon.transform);
            }
        }
    }
}