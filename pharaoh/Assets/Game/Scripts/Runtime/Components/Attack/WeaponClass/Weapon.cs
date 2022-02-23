using System;
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

        public UnityEvent onWeaponThrown = new UnityEvent();
        public UnityEvent onGroundHit = new UnityEvent();
        public UnityEvent<Transform> onSocketAttach = new UnityEvent<Transform>();

        private Transform _parent = null;

        private void OnEnable()
        {
            onSocketAttach?.AddListener(SocketAttach);
        }

        private void OnDisable()
        {
            onSocketAttach?.RemoveListener(SocketAttach);
        }

        public void Update()
        {
            if (_parent != transform.parent)
            {
                onSocketAttach?.Invoke(transform.parent);
            }
        }

        private void SocketAttach(Transform socket)
        {
            if (!rigidbody)
            {
                LogHandler.SendMessage($"Can't socket damager.", MessageType.Warning);
                return;
            }

            _parent = socket;
            rigidbody.isKinematic = socket;
            rigidbody.useGravity = !socket;

            isOnGround = false;
            isThrown = !socket;
            if (!isThrown && !isOnGround)
            {
                collider.isTrigger = true;
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
                rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }
        }

        public void Throw(Damager damager)
        {
            if (damager != this || !TryGetComponent(out Ballistic ballistic)) return;

            transform.parent = null;
            onWeaponThrown?.Invoke();
        }
    }
}