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

        private void FixedUpdate()
        {
            if (isThrown)
            {
                _rigidbody?.AddForce(Vector3.up * (gravity * -2f));
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.IsInLayerMask(collidingLayers))
            {
                isThrown = false;
            }
        }

        public void Parenting(Transform parent = null)
        {
            transform.parent = parent;
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
            isThrown = true;
        }
    }
}