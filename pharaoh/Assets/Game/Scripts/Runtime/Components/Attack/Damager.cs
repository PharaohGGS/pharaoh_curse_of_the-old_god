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
        [HideInInspector] public UnityEvent<Damager, Collider2D> onDamagingHit;

        [SerializeField] private LayerMask collidingLayers;
        [HideInInspector] public UnityEvent<Damager, Collider2D> onCollisionHit;

        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.IsCollidingHimself(other, true)) return;

            enterFirstContactPosition = _collider.ClosestPoint(transform.TransformPoint(other.offset));

            if (other.gameObject.HasLayer(damagingLayers))
            {
                onDamagingHit?.Invoke(this, other);
            }

            if (other.gameObject.HasLayer(collidingLayers))
            {
                // kinematic + sleep
                onCollisionHit?.Invoke(this, other);
            } 
        }
    }
}
