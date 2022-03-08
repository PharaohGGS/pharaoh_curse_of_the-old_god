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
            if (!t.TryGetComponent(out Gear gear)) return state;
            if (!gear.isThrown || !gear.isGrounded) return state;
            if (!_attack.TryGetHolder(gear.GetBaseData(), out GearHolder holder)) return state;

            gear.transform.parent = holder.transform;
            gear.transform.localPosition = Vector3.zero;
            gear.transform.localRotation = Quaternion.Euler(0, 0, 0);
            blackboard.ClearData("target");

            //if (gear.TryGetData(out MeleeGearData meleeGearData))
            //{
            //    blackboard.SetData("isWaiting", true);
            //    blackboard.SetData("waitTime", meleeGearData.throwablePickingTime);
            //}

            state = NodeState.Success;
            return state;
        }
    }
}