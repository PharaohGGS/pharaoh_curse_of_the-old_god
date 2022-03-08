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
        public IObjectPool<Damager> ammos { get; private set; }

        private Transform _currentTarget = null;

        protected override void Awake()
        {
            base.Awake();
            ammos = new LinkedPool<Damager>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool,
                OnDestroyPoolObject, collectionChecks, maxPoolSize);
        }

        private void OnDestroyPoolObject(Damager damager)
        {
            Destroy(damager.gameObject);
        }

        private void OnReturnedToPool(Damager damager)
        {
            damager.gameObject.SetActive(false);
            damager.transform.localPosition = Vector3.zero;
            damager.transform.localRotation = Quaternion.identity;
        }

        private void OnTakeFromPool(Damager damager)
        {
            damager.gameObject.SetActive(true);
        }

        private Damager CreatePoolItem()
        {
            var damager = GameObject.Instantiate(damagerPrefab, shootStart.position, Quaternion.identity, transform);

            var returnToPool = damager.gameObject.AddComponent<DamagerReturnToPool>();
            returnToPool.pool = ammos;

            return damager;
        }

        public void SetupShoot(Gear shootingGear, Transform target)
        {
            if (shootingGear != this || !target) return;

            _currentTarget = target;
        }
        
        public void Shoot(Gear shootingGear)
        {
            if (shootingGear != this) return;

            var damager = ammos.Get();

            if (damager.TryGetComponent(out Rigidbody2D rb2D))
            {
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                var direction = (Vector2)_currentTarget.position - rb2D.position;
                rb2D.AddForce(direction.normalized * GetData().shootInitialVelocity, ForceMode2D.Impulse);
            }

            onGearShoot?.Invoke(damager);

            LogHandler.SendMessage($"{shootingGear.name} shooting {damager.name}", MessageType.Warning);
        }
    }
}