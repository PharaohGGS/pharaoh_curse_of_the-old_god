using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Damager : MonoBehaviour
    {
        public DamagerData data;
        public LayerMask damagingLayers;
        public LayerMask collidingLayers;
        public UnityEvent<Damager> OnHit;

        public Collider lastTriggerEnter { get; protected set; }
        public Rigidbody rigidBody { get; protected set; }
        public Collider collider { get; protected set; }

        protected virtual void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            rigidBody.useGravity = false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == gameObject) return;
            
            if (!other.gameObject.IsInLayerMask(damagingLayers)) return;

            lastTriggerEnter = other;
            OnHit?.Invoke(this);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            lastTriggerEnter = null;
        }
    }
}
