using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class AttackComponent : MonoBehaviour
    {
        [field: SerializeField] public Gear[] gears { get; private set; }
        public readonly Dictionary<GearData, Gear> dataGears = new Dictionary<GearData, Gear>();

        public HealthComponent currentTargetHealth { get; private set; }

        public UnityEvent<Gear, GameObject> onGearAttack = new UnityEvent<Gear, GameObject>();
        
        private void Awake()
        {
            if (gears.Length <= 0)
            {
                gears = GetComponentsInChildren<Gear>();
            }

            foreach (var gear in gears)
            {
                dataGears.TryAdd(gear.GetBaseData(), gear);
            }
        }

        public void Attack(GearData data, GameObject target)
        {
            if (!data || !target || !dataGears.TryGetValue(data, out Gear gear)) return;
            
            if (currentTargetHealth?.gameObject != target && target.TryGetComponent(out HealthComponent targetHealth))
            {
                currentTargetHealth?.onDeath?.RemoveListener(OnTargetDeath);
                currentTargetHealth = targetHealth;
                currentTargetHealth?.onDeath?.AddListener(OnTargetDeath);
            }

            onGearAttack?.Invoke(gear, target);
        }

        private void OnTargetDeath(HealthComponent arg0)
        {
            currentTargetHealth = null;
        }

        //public bool TryGetHolder(GearData data, out GearHolder holder)
        //{
        //    holder = null;
        //    if (holders.Length <= 0) return false;
        //    foreach (var h in holders)
        //    {
        //        var hData = h.gear != null ? h.gear.GetBaseData() : null;
        //        if (hData == null || hData != data) continue;

        //        holder = h;
        //        return true;
        //    }

        //    return false;
        //}

        //public bool TryGetHolder(GearType type, out GearHolder holder)
        //{
        //    holder = null;
        //    if (holders.Length <= 0) return false;
        //    foreach (var h in holders)
        //    {
        //        var hData = h.gear != null ? h.gear.GetBaseData() : null;
        //        if (hData == null || hData.GetGearType() != type) continue;

        //        holder = h;
        //        return true;
        //    }

        //    return false;
        //}
    }
}