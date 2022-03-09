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
            //_damager?.onTriggerHit?.AddListener(Release);
            _damager?.onCollidingHit?.AddListener(Release);
        }

        private void OnDisable()
        {
            //_damager?.onTriggerHit?.RemoveListener(Release);
            _damager?.onCollidingHit?.RemoveListener(Release);
        }

        private void Release(Damager hitDamager)
        {
            if (hitDamager != _damager) return;

            pool?.Release(hitDamager);
        }
    }
}