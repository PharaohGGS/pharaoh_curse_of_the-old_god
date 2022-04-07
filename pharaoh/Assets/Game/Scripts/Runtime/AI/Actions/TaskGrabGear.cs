using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGrabGear : ActionNode
    {
        private FightComponent _fight = null;
        
        protected override void OnStart()
        {
            if (_fight) return;

            if (!agent.TryGetComponent(out _fight))
            {
                LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
            }
        }

        protected override NodeState OnUpdate()
        {
            if (!_fight || !blackboard.TryGetData("target", out Transform t)) return NodeState.Running;
            if (!t.TryGetComponent(out Gear gear) || !gear.isThrown || !gear.isGrounded) return NodeState.Running;

            gear.SocketAttach(true);
            blackboard.ClearData("target");
            
            var weapon = _fight.activeWeapon;
            if (weapon && weapon.isActiveAndEnabled)
            {
                blackboard.SetData("isWaiting", true);
                blackboard.SetData("waitTime", weapon.GetBaseData().rate);
            }   
            
            return NodeState.Success;
        }
    }
}