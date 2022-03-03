using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Pharaoh.Gameplay.Components
{
    public class DistanceGear : Gear<DistanceGearData>
    {
        // weapon bullet pool
        [SerializeField] private Damager damagerPrefab;
        [SerializeField] private bool collectionChecks;
        [SerializeField] private int maxPoolSize;

        [SerializeField] private Transform shootStart;

        private IObjectPool<Damager> _pool;
        public IObjectPool<Damager> pool 
        { 
            get
            {
                if (_pool == null)
                {
                    _pool = new LinkedPool<Damager>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool,
                        OnDestroyPoolObject, collectionChecks, maxPoolSize);
                }

                return _pool;
            }
        }

        private void OnDestroyPoolObject(Damager damager)
        {
            Destroy(damager.gameObject);
        }

        private void OnReturnedToPool(Damager damager)
        {
            damager.gameObject.SetActive(false);
        }

        private void OnTakeFromPool(Damager damager)
        {
            damager.gameObject.SetActive(true);
        }

        private Damager CreatePoolItem()
        {
            var damager = GameObject.Instantiate(damagerPrefab, shootStart.position, Quaternion.identity, transform);

            var returnToPool = damager.gameObject.AddComponent<DamagerReturnToPool>();
            returnToPool.pool = pool;

            return damager;
        }
        
        public void Shoot(Damager shootDamager)
        {
            //if (shootDamager != damager || !damager.data.throwable ||
            //    !TryGetComponent(out Ballistic ballistic)) return;

            //transform.parent = null;
            //onWeaponThrown?.Invoke();
        }
    }
}