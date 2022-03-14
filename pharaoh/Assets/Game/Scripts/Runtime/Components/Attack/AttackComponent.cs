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

        public UnityEvent<Gear> onGearAttack = new UnityEvent<Gear>();
        public UnityEvent<Gear, Transform> onGearAimTarget = new UnityEvent<Gear, Transform>();

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
            
            onGearAimTarget?.Invoke(holder.gear, target);
            onGearAttack?.Invoke(holder.gear);
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

        public bool TryGetHolder(GearType type, out GearHolder holder)
        {
            holder = null;
            if (holders.Length <= 0) return false;
            foreach (var h in holders)
            {
                var hData = h.gear != null ? h.gear.GetBaseData() : null;
                if (hData == null || hData.GetGearType() != type) continue;

                holder = h;
                return true;
            }

            return false;
        }
    }
}