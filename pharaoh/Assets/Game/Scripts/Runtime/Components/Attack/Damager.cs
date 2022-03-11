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
        [field: SerializeField] public DamagerData data { get; private set; }
        private Collider2D _collider2D;

        public LayerMask damagingLayers;
        public UnityEvent<Damager, Collider2D> onTriggerHit;

        public LayerMask collidingLayers;
        [HideInInspector] public UnityEvent<Damager, Collision2D> onCollisionHit;

        [HideInInspector, SerializeField] private int maxOverlappedColliders = 3;
        [HideInInspector] public UnityEvent<Damager, Collider2D[]> onOverlapHit;
        private Collider2D[] _overlappedColliders;
        
        protected virtual void Awake()
        {
            _overlappedColliders = new Collider2D[maxOverlappedColliders];
            if (!_collider2D) _collider2D = GetComponent<Collider2D>();
        }

        protected virtual void FixedUpdate()
        {
            if (collidingLayers.value <= 0 || !_collider2D) return;
            // equivalent to OnCollisionEnter2D with trigger
            var size = _collider2D.OverlapNonAlloc(ref _overlappedColliders, collidingLayers);
            if (size > 0) onOverlapHit?.Invoke(this, _overlappedColliders);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == gameObject) return;
            
            if (other.gameObject.HasLayer(damagingLayers) && !gameObject.IsCollidingHimself(other, true))
            {
                onTriggerHit?.Invoke(this, other);
            }
        }

        /// <summary>
        /// Not working when trigger engaged
        /// </summary>
        /// <param name="col"></param>
        protected void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject == gameObject) return;

            if (col.gameObject.HasLayer(collidingLayers))
            {
                onCollisionHit?.Invoke(this, col);
            }
        }
    }
}
