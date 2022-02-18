using System;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class Weapon : Damager
    {
        [SerializeField] private float gravity = 9.81f;
        [SerializeField] private float height = 2f;

        public bool isThrown { get; private set; }
        public bool isOnGround { get; private set; }

        private void FixedUpdate()
        {
            if (isThrown)
            {
                _rigidbody?.AddForce(Vector3.up * (gravity * -2f));
                //_rigidbody.rotation = Quaternion.LookRotation(_rigidbody.velocity.normalized, Vector3.up);
                _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation,
                    Quaternion.LookRotation(_rigidbody.velocity.normalized, Vector3.up)/* * Quaternion.Euler(90, 0, 0)*/,
                    _rigidbody.velocity.magnitude /** Time.fixedDeltaTime*/);
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (isThrown && other.gameObject.IsInLayerMask(collidingLayers))
            {
                _collider.isTrigger = false;
                return;
            }

            base.OnTriggerEnter(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.IsInLayerMask(collidingLayers))
            {
                isOnGround = true;
                _rigidbody.velocity = _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
                _rigidbody.useGravity = false;
            }
        }

        public void Parenting(Transform parent = null)
        {
            transform.parent = parent;
            isThrown = transform.parent == null;
            isOnGround = false;

            if (!isOnGround && !isThrown)
            {
                _collider.isTrigger = true;
            }
        }

        public void Throw(Vector3 target)
        {
            if (!data.canThrow || _rigidbody == null)
            {
                LogHandler.SendMessage($"Can't throw this weapon!", MessageType.Warning);
                return;
            }

            var launchData = LaunchData.Calculate(gravity, height, target, _rigidbody.position);
            
            Parenting();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.velocity = launchData.initialVelocity;
        }
    }
}