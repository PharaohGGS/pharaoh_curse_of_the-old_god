using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class FightComponent : MonoBehaviour
    {
        [Header("Gears"), SerializeField] 
        private Gear[] gears;

        [Header("Armors")]
        private readonly List<Gear> _armors = new List<Gear>();
        public float armorDeal
        {
            get
            {
                if (_armors.Count <= 0) return 0f;

                float deal = 0f;
                foreach (var gear in _armors)
                {
                    if (!gear.transform.parent || !gear.TryGetData(out DefenseGearData defenseData)) continue;
                    deal += defenseData.deal;
                }

                return deal;
            }
        }

        [Header("Weapons")]
        private int _weaponIndex = -1;
        private readonly List<Gear> _weapons = new List<Gear>();
        [field: SerializeField, Range(0.5f, 10f)] public float attackRange { get; private set; } = 5f;
        public Gear activeWeapon => _weapons.Count >= 1 ? _weapons[_weaponIndex] : null;
        private HealthComponent _currentTargetHealth;
        public bool hasTarget => _currentTargetHealth != null;
        
        private void Awake()
        {
            if (gears.Length <= 0)
            {
                gears = GetComponentsInChildren<Gear>();
            }

            foreach (var gear in gears)
            {
                var data = gear.GetBaseData();
                if (data == null) continue;

                if (data.GetGearType() == GearType.Defense) continue;
                _weapons.Add(gear);
            }

            if (_weapons.Count <= 0) return;
            _weaponIndex = 0;
        }

        public void ChangeWeapon()
        {
            if (_weapons.Count <= 1) return;
            _weaponIndex = (_weaponIndex + 1) % _weapons.Count;
        } 

        public void Attack(GameObject target)
        {
            if (!activeWeapon || !target) return;

            if (target.TryGetComponent(out HealthComponent targetHealth))
            {
                SubscribeToTargetHealth(targetHealth);
            }

            if (activeWeapon is IWeapon weapon) weapon.Attack(target);
        }

        private void SubscribeToTargetHealth(HealthComponent targetHealth)
        {
            if (_currentTargetHealth == targetHealth) return;
            _currentTargetHealth?.onDeath?.RemoveListener(OnTargetDeath);
            _currentTargetHealth = targetHealth;
            _currentTargetHealth?.onDeath?.AddListener(OnTargetDeath);
        }

        private void OnTargetDeath(HealthComponent targetHealth)
        {
            _currentTargetHealth = null;
        }
    }
}