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
            _damager?.onHit?.AddListener(Release);
        }

        private void OnDisable()
        {
            _damager?.onHit?.RemoveListener(Release);
        }

        private void Release(Damager hitDamager)
        {
            if (hitDamager != _damager) return;

            pool?.Release(hitDamager);
        }
    }
}