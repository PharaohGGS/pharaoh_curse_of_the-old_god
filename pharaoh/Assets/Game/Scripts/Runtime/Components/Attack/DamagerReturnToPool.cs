using System;
using System.Collections;
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
            _damager?.onDamagingHit?.AddListener(TriggerHit);
            _damager?.onCollisionHit?.AddListener(CollisionHit);
        }
        
        private void OnDisable()
        {
            _damager?.onDamagingHit?.RemoveListener(TriggerHit);
            _damager?.onCollisionHit?.RemoveListener(CollisionHit);
            StopAllCoroutines();
        }
        
        private void TriggerHit(Damager damager, Collision2D collision) => Release(damager);
        private void CollisionHit(Damager damager, Collision2D collision) => Release(damager);

        public void StartLifeTimeCountDown(float lifeTime)
        {
            StartCoroutine(TimeLeftBeforeRelease(lifeTime));
        }

        private IEnumerator TimeLeftBeforeRelease(float lifeTime)
        {
            yield return new WaitForSeconds(lifeTime);
            Release(_damager);
        }

        private void Release(Damager damager)
        {
            if (damager != _damager) return;

            pool?.Release(damager);
        }
    }
}