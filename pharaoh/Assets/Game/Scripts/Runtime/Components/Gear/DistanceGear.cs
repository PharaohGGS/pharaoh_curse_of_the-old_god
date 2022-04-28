using System;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(DamagerPool))]
    public class DistanceGear : Gear<DistanceGearData>, IWeapon
    {
        private DamagerPool _pool;

        // weapon bullet pool
        protected override void Awake()
        {
            base.Awake();
            _pool = GetComponent<DamagerPool>();
        }
        
        public void Shoot(Vector3 target)
        {
            var damager = _pool.Get();

            if (damager.TryGetComponent(out Rigidbody2D rb2D))
            {
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                var direction = (Vector2)target - rb2D.position;
                rb2D.AddForce(direction.normalized * GetData().shootInitialVelocity, ForceMode2D.Impulse);
            }

            LogHandler.SendMessage($"{name} shooting {damager.name}", MessageType.Warning);
        }
    }
}