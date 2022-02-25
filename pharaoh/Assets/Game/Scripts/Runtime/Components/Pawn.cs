using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Rigidbody), typeof(MovementComponent), typeof(DetectionComponent))]
    public class Pawn : Actor
    {
        public HealthComponent health { get; protected set; }
        public MovementComponent movement { get; protected set; }
        public DetectionComponent detection { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
            movement = GetComponent<MovementComponent>();
            detection = GetComponent<DetectionComponent>();

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