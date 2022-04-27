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
        [field: SerializeField] public DamagerData data { get; set; }
        public StunData stunData;
        public LayerMask ownerMask;
        
        [SerializeField] private LayerMask damagingLayers;
        [HideInInspector] public UnityEvent<Damager, Collider2D> onDamagingHit;

        [SerializeField] private LayerMask collidingLayers;
        [HideInInspector] public UnityEvent<Damager, Collider2D> onCollisionHit;

        private Collider2D _collider;
        public Transform owner { get; private set; }

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();

            if (ownerMask.value <= 0) return;
            // get the owner by parent
            Transform tr = transform.parent;
            while (tr != transform.root)
            {
                if (tr.gameObject.HasLayer(ownerMask)) break;
                tr = tr.parent;
            }

            owner = tr;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.IsCollidingHimself(other, true)) return;
            
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
