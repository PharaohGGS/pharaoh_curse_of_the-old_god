using System;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Game.Scripts.Runtime.Components
{
    [RequireComponent(typeof(Rigidbody), typeof(MovementComponent))]
    public class Pawn : MonoBehaviour
    {
        public Collider[] colliders { get; protected set; }
        public Rigidbody physicsBody { get; protected set; }
        public HealthComponent health { get; protected set; }
        public MovementComponent movement { get; protected set; }

        protected virtual void OnEnable()
        {
            colliders = GetComponents<Collider>();
            physicsBody = GetComponent<Rigidbody>();
            movement = GetComponent<MovementComponent>();

            if (health == null && TryGetComponent(out HealthComponent hlth)) health = hlth;
        }

        public void TakeHit(Damager damager)
        {
            if (colliders.Length == 0) return;

            foreach (var col in colliders)
            {
                if (col == damager.lastTriggerEnter)
                {
                    LogHandler.SendMessage($"Take hit from {damager}", MessageType.Log);
                    health.Decrease(damager.data.damage);
                    break;
                }
            }
        }
    }
}