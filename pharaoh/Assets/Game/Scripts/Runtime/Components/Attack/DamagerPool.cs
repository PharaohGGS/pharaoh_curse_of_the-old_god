using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Pharaoh.Gameplay.Components
{
    public class DamagerPool : MonoBehaviour
    {
        [SerializeField] private bool collectionChecks;
        [SerializeField] private int maxPoolSize;
        [SerializeField] private Damager damagerPrefab;
        [SerializeField] private Transform shootStart;
        [SerializeField] private UnityEvent<Damager> onGearShoot = new UnityEvent<Damager>();

        private IObjectPool<Damager> _pool;

        private void Awake()
        {
            if (shootStart == null) shootStart = transform;
            _pool = new LinkedPool<Damager>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool,
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
            returnToPool.pool = _pool;

            return damager;
        }

        public Damager Get()
        {
            var damager = _pool.Get();
            onGearShoot?.Invoke(damager);
            return damager;
        }
    }
}