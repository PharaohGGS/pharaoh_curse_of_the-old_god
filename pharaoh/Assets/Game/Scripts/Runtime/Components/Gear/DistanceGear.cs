using System;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;
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

        [HideInInspector] public UnityEvent<Damager> onGearShoot = new UnityEvent<Damager>();

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
        
        public void Shoot(Gear shootingGear)
        {
            if (shootingGear != this) return;

            var damager = pool.Get();

            LogHandler.SendMessage($"{shootingGear.name} shooting {damager.name}", MessageType.Warning);
        }
    }
}