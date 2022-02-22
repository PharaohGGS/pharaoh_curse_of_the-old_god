using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGrabWeapon : ActionNode
    {
        private Pawn _pawn = null;
        private DamagerHolder _holder = null;

        protected override void OnStart()
        {
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
                return;
            }

            if (_holder) return;

            var holder = agent.TryGetComponent(out DamagerHolder h)
                ? h : agent.GetComponentInChildren<DamagerHolder>();

            if (!holder?.damager)
            {
                LogHandler.SendMessage($"[{agent.name}] Can't attack enemies", MessageType.Warning);
                return;
            }

            _holder = holder;
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;
            var damager = _holder.damager;

            if (damager is not Weapon weapon) return state;
            if (!weapon.isThrown || !weapon.isOnGround) return state;

            var detection = _pawn.detection;

            var overlap = Physics.OverlapSphere(agent.transform.position, 
                detection.pickupRange, detection.weaponLayer);
            
            if (overlap.Length <= 0) return state;

            if (overlap.Length > 1)
            {
                Array.Sort(overlap, (x, y) =>
                    Vector3.Distance(agent.transform.position, x.transform.position).CompareTo(
                        Vector3.Distance(agent.transform.position, y.transform.position)));
            }

            foreach (var collider in overlap)
            {
                if (!collider.TryGetComponent(out Weapon w) || w != weapon) continue;
                
                weapon.transform.parent = _holder.transform;
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _pawn.rigidBody.velocity = _pawn.rigidBody.angularVelocity = Vector3.zero;
                blackboard.ClearData("target");
                blackboard.SetData("isWaiting", true);
                blackboard.SetData("waitTime", _holder.timeAfterPickingWeapon);
                state = NodeState.Success;
                break;
            }
            
            return state;
        }
    }
}