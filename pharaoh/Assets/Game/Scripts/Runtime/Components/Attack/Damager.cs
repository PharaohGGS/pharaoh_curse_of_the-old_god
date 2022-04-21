using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Collider2D))]
    public class Damager : MonoBehaviour
    {
        public DamagerData damagerData;
        public StunData stunData;

        public Vector2 enterFirstContactPosition { get; private set; }

        [SerializeField] private LayerMask damagingLayers;
        [HideInInspector] public UnityEvent<Damager, Collision2D> onDamagingHit;

        [SerializeField] private LayerMask collidingLayers;
        [HideInInspector] public UnityEvent<Damager, Collision2D> onCollisionHit;

        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"{collision.collider}");
            if (gameObject.IsCollidingHimself(collision.collider, true)) return;

            enterFirstContactPosition = collision.GetContact(0).point;

            if (collision.collider.gameObject.HasLayer(damagingLayers))
            {
                onDamagingHit?.Invoke(this, collision);
            }

            if (collision.collider.gameObject.HasLayer(collidingLayers))
            {
                // kinematic + sleep
                onCollisionHit?.Invoke(this, collision);
            } 
        }
    }
}
