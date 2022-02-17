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
        protected Rigidbody _rigidbody;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == gameObject) return;

            if (other.gameObject.IsInLayerMask(collidingLayers))
            {
                if (TryGetComponent(out Collider col))
                {
                    col.isTrigger = false;
                }

                return;
            }

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
