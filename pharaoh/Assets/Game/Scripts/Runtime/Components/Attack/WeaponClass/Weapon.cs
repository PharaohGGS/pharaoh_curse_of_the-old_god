using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;
using Void = Pharaoh.Gameplay.Components.Events.Void;

namespace Pharaoh.Gameplay.Components
{
    public class Weapon : Damager
    {
        public LayerMask collidingLayers;
        public bool isThrown { get; protected set; }
        public bool isOnGround { get; protected set; }

        public UnityEvent onThrown;
        public UnityEvent onGroundHit;

        private Transform _attach;
        public Transform attach
        {
            get => _attach;
            set
            {
                if (!rigidBody)
                {
                    LogHandler.SendMessage($"Can't attach weapon.", MessageType.Warning);
                    return;
                }
                
                isOnGround = false;
                transform.parent = value;
                isThrown = rigidBody.isKinematic = value;
                rigidBody.useGravity = !value;

                if (!isOnGround && !isThrown)
                {
                    collider.isTrigger = true;
                }
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (isThrown && other.gameObject.IsInLayerMask(collidingLayers))
            {
                collider.isTrigger = false;
                return;
            }

            base.OnTriggerEnter(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.IsInLayerMask(collidingLayers))
            {
                isOnGround = true;
                onGroundHit?.Invoke();
                rigidBody.velocity = rigidBody.angularVelocity = Vector3.zero;
                rigidBody.isKinematic = true;
                rigidBody.useGravity = false;
            }
        }
    }
}