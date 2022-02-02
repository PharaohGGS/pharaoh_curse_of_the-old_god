using System;
using Pharaoh.Gameplay.Component;
using UnityEngine;

namespace Pharaoh.Test
{
    [RequireComponent(typeof(DamageComponent), typeof(Collider), typeof(Rigidbody))]
    public class TestDamage : MonoBehaviour
    {
        [SerializeField] private DamageComponent damageComponent;

        private void Start()
        {
            if (!TryGetComponent(out damageComponent))
            {
                Debug.LogWarning($"Doesn't have a Damage Component.");
            }
        }

        private void OnCollisionStay(Collision collisionInfo)
        {
            if (collisionInfo.gameObject != null)
            {
                damageComponent.Hit(collisionInfo.gameObject);
            }
        }
    }
}