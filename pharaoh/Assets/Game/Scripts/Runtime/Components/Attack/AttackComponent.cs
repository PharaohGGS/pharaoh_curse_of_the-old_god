using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class AttackComponent : MonoBehaviour
    {
        [SerializeField] private GearHolder[] holders;
        public Transform target { get; set; }

        public UnityEvent<Gear> onDamagerAttack = new UnityEvent<Gear>();
        public UnityEvent<Transform> onDamagerAimTarget = new UnityEvent<Transform>();

        private void Awake()
        {
            if (holders.Length <= 0)
            {
                holders = GetComponentsInChildren<GearHolder>();
            }
        }

        public void Attack(GearHolder holder)
        {
            if (!holder || !holder.gear) return;
            
            onDamagerAimTarget?.Invoke(target);
            onDamagerAttack?.Invoke(holder.gear);
        }

        public bool ContainsHolder(GearData data)
        {
            if (holders.Length <= 0) return false;

            foreach (var h in holders)
            {
                var hData = h.gear != null ? h.gear.GetBaseData() : null;
                if (hData == null || hData != data) continue;
                return true;
            }

            return false;
        }

        public bool TryGetHolder(GearData data, out GearHolder holder)
        {
            holder = null;
            if (holders.Length <= 0) return false;
            foreach (var h in holders)
            {
                var hData = h.gear != null ? h.gear.GetBaseData() : null;
                if (hData == null || hData != data) continue;

                holder = h;
                return true;
            }

            return false;
        }
    }
}