using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

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
                if (!rigidbody)
                {
                    LogHandler.SendMessage($"Can't attach damager.", MessageType.Warning);
                    return;
                }
                
                isOnGround = false;
                _attach = transform.parent = value;
                rigidbody.isKinematic = value;
                rigidbody.useGravity = isThrown = !value;

                if (!isOnGround && !isThrown)
                {
                    collider.isTrigger = true;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            attach = transform.parent;
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
                rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }
        }
    }
}