using System;
using BehaviourTree.Tools;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.AI.Actions
{
    public class TaskGrabWeapon : ActionNode
    {
        private EnemyPawn _pawn = null;
        private WeaponHolder _holder = null;

        protected override void OnStart()
        {
            if (_pawn == null && !agent.TryGetComponent(out _pawn))
            {
                LogHandler.SendMessage($"Not a pawn !", MessageType.Error);
                return;
            }

            if (_holder) return;

            if (_holder == null && !_pawn.holder)
            {
                LogHandler.SendMessage($"Can't have a weapon !", MessageType.Error);
                return;
            }

            _holder = _pawn.holder;
        }

        protected override NodeState OnUpdate()
        {
            state = NodeState.Running;
            var weapon = _holder.weapon;

            if (weapon && weapon.isThrown && weapon.isOnGround)
            {
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
                    weapon.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    state = NodeState.Success;
                    break;
                }
            }
            
            return state;
        }
    }
}