using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class AttackComponent : MonoBehaviour
    {
        [SerializeField] private WeaponHolder[] holders;
        public Transform target { get; set; }

        public UnityEvent<Gear> onDamagerAttack = new UnityEvent<Gear>();
        public UnityEvent<Gear, Transform> onDamagerAimTarget = new UnityEvent<Gear, Transform>();

        private void Awake()
        {
            if (holders.Length <= 0)
            {
                holders = GetComponentsInChildren<WeaponHolder>();
            }
        }

        public void Attack(WeaponHolder holder)
        {
            if (holder == null) return;
            
            onDamagerAimTarget?.Invoke(holder.Gear, target);
            onDamagerAttack?.Invoke(holder.Gear);
        }

        public bool TryGetHolder(DamagerData data, out WeaponHolder holder)
        {
            holder = null;
            if (holders.Length <= 0) return false;
            foreach (var h in holders)
            {
                if (h.data == null || h.data != data) continue;

                holder = h;
                return true;
            }

            return false;
        }

        public bool TryGetHolder<T>(out WeaponHolder holder) where T : DamagerData
        {
            holder = null;
            if (holders.Length <= 0) return false;
            foreach (var h in holders)
            {
                if (h.data == null || h.data.GetType() != typeof(T)) continue;

                holder = h;
                return true;
            }

            return false;
        }
    }
}