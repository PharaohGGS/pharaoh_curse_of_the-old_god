using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(DamageComponent), typeof(Rigidbody))]
    public class Weapon : MonoBehaviour
    {
        public WeaponData data;
        public LayerMask touchingLayers;
        public UnityEvent<Collider> OnWeaponHit;

        [SerializeField] private float height = 2f;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == touchingLayers)
            {
                OnWeaponHit?.Invoke(other);
            }
        }

        public void Throw(Vector3 target)
        {
            if (!data.canThrow || _rigidbody == null) return;

            var gravity = Physics.gravity.magnitude;
            var launchData = LaunchData.Calculate(gravity, height, target, _rigidbody.position);
            Physics.gravity = Vector3.up * gravity * -2f;
            _rigidbody.useGravity = true;
            _rigidbody.velocity = launchData.initialVelocity;
        }
    }
}