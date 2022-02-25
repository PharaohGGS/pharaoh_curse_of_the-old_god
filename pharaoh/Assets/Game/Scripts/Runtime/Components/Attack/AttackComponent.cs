using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class AttackComponent : MonoBehaviour
    {
        [SerializeField] private DamagerHolder[] holders;
        public Transform target { get; set; }

        public UnityEvent<Damager> onDamagerAttack = new UnityEvent<Damager>();
        public UnityEvent<Damager, Transform> onDamagerAimTarget = new UnityEvent<Damager, Transform>();

        private void Awake()
        {
            if (holders.Length <= 0)
            {
                holders = GetComponentsInChildren<DamagerHolder>();
            }
        }

        public void Attack(DamagerHolder holder)
        {
            if (holder == null) return;
            
            onDamagerAimTarget?.Invoke(holder.damager, target);
            onDamagerAttack?.Invoke(holder.damager);
        }

        public bool TryGetHolder<T>(out DamagerHolder holder) where T : DamagerData
        {
            holder = null;
            if (holders.Length <= 0) return false;

            foreach (var h in holders)
            {
                if (h.data.GetType() != typeof(T)) continue;

                holder = h;
                return true;
            }

            return false;
        }
    }
}