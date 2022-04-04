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

        [SerializeField] private LayerMask damagingLayers;
        [HideInInspector] public UnityEvent<Damager, Collider2D> onTriggerHit;

        [SerializeField] private LayerMask collidingLayers;
        [HideInInspector] public UnityEvent<Damager, Collider2D> onCollisionHit;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gameObject.IsCollidingHimself(other, true)) return;
            
            if (other.gameObject.HasLayer(damagingLayers))
            {
                onTriggerHit?.Invoke(this, other);
            }

            if (other.gameObject.HasLayer(collidingLayers))
            {
                // kinematic + sleep
                onCollisionHit?.Invoke(this, other);
            } 
        }
    }
}
