using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGrabWeapon : ActionNode
    {
        private AttackComponent _attack = null;
        
        protected override void OnStart()
        {
            if (_attack) return;

            if (!agent.TryGetComponent(out _attack))
            {
                LogHandler.SendMessage($"[{agent.name}] Can't _attack enemies", MessageType.Warning);
            }
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;
            
            if (!_attack || !blackboard.TryGetData("target", out Transform t)) return state;
            if (!t.TryGetComponent(out Gear weapon)) return state;
            if (!weapon.isThrown || !weapon.isGrounded) return state;
            if (!_attack.TryGetHolder(weapon.data, out WeaponHolder holder)) return state;

            weapon.transform.parent = holder.transform;
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
            blackboard.ClearData("target");
            blackboard.SetData("isWaiting", true);
            blackboard.SetData("waitTime", weapon.data.throwablePickingTime);
            state = NodeState.Success;
            return state;
        }
    }
}