using System;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Pool;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Damager))]
    public class DamagerReturnToPool : MonoBehaviour
    {
        public IObjectPool<Damager> pool;
        private Damager _damager;

        private void Awake()
        {
            _damager = GetComponent<Damager>();
        }

        private void OnEnable()
        {
            _damager?.onTriggerHit?.AddListener(TriggerHit);
            _damager?.onOverlapHit?.AddListener(OverlapHit);
            _damager?.onCollisionHit?.AddListener(CollisionHit);
        }
        
        private void OnDisable()
        {
            _damager?.onTriggerHit?.RemoveListener(TriggerHit);
            _damager?.onOverlapHit?.RemoveListener(OverlapHit);
            _damager?.onCollisionHit?.RemoveListener(CollisionHit);
        }
        
        private void TriggerHit(Damager damager, Collider2D collider2D) => Release(damager);
        private void CollisionHit(Damager damager, Collision2D collision2D) => Release(damager);
        private void OverlapHit(Damager damager, Collider2D[] collider2Ds) => Release(damager);

        private void Release(Damager damager)
        {
            if (damager != _damager) return;

            pool?.Release(damager);
        }
    }
}