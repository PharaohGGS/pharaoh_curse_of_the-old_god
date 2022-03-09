using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Damager : MonoBehaviour
    {
        [SerializeField] private DamagerData data;
        public LayerMask damagingLayers;
        [HideInInspector] public UnityEvent<Damager> onTriggerHit;

        public LayerMask collidingLayers;
        [HideInInspector] public UnityEvent<Damager> onCollidingHit = new UnityEvent<Damager>();

        public Rigidbody2D rb2D { get; protected set; }
        public Collider2D coll2D { get; protected set; }
        public Collider2D lastTriggerEnter { get; protected set; }

        public DamagerData GetData() => data;

        [SerializeField] private int maxOverlapedColliders = 3;
        private Collider2D[] _overlapedColliders;

        protected virtual void Awake()
        {
            _overlapedColliders = new Collider2D[maxOverlapedColliders];
            rb2D = GetComponent<Rigidbody2D>();
            if (TryGetComponent(out Collider2D collider2D)) coll2D = collider2D;
            rb2D.bodyType = RigidbodyType2D.Kinematic;
        }

        protected virtual void FixedUpdate()
        {
            // equivalent to OnCollisionEnter2D with trigger
            var size = coll2D.OverlapNonAlloc(ref _overlapedColliders, collidingLayers);
            if (size > 0) onCollidingHit?.Invoke(this);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == gameObject) return;
            
            if (other.gameObject.IsInLayerMask(damagingLayers))
            {
                var parentColliders = GetComponentsInParent<Collider2D>();

                // to exclude all potential collider of the gameObject holding this damager
                if (parentColliders.Length > 0)
                {
                    foreach (var collider in parentColliders)
                    {
                        if (collider.gameObject == other.gameObject) return;
                    }
                }

                lastTriggerEnter = other;
                onTriggerHit?.Invoke(this);
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            lastTriggerEnter = null;
        }
    }
}
