using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class FightComponent : MonoBehaviour
    {
        [Header("Weapons")]

        [SerializeField] private Gear[] gears;
        [field: SerializeField, Range(0.5f, 10f)] public float range { get; private set; } = 5f;

        private int _weaponIndex = -1;
        private readonly List<Gear> _weapons = new List<Gear>();
        
        private Transform _currentTarget;
        private HealthComponent _targetHealth;

        public Gear activeWeapon => _weapons.Count >= 1 ? _weapons[_weaponIndex] : null;
        public bool hasTarget => _currentTarget != null;
        
        private void Awake()
        {
            if (gears.Length <= 0)
            {
                gears = GetComponentsInChildren<Gear>();
            }

            foreach (var gear in gears) if (gear is IWeapon) _weapons.Add(gear);
            if (_weapons.Count <= 0) return;
            _weaponIndex = 0;
        }

        public void ChangeWeapon()
        {
            if (_weapons.Count <= 1) return;
            _weaponIndex = (_weaponIndex + 1) % _weapons.Count;
        } 

        public void Attack(Transform target)
        {
            if (!activeWeapon || !target) return;

            if (_currentTarget != target)
            { 
                _currentTarget = target;
                SubscribeToTargetHealth();
            }

            if (activeWeapon is IWeapon weapon)
            {
                weapon.Attack(target);
            }
        }

        private void SubscribeToTargetHealth()
        {
            if (!_currentTarget.TryGetComponent(out HealthComponent health) || health == _targetHealth)
            {
                return;
            }   
            
            _targetHealth?.onDeath?.RemoveListener(OnTargetDeath);
            _targetHealth = health;
            _targetHealth?.onDeath?.AddListener(OnTargetDeath);
        }

        private void OnTargetDeath(HealthComponent targetHealth)
        {
            _targetHealth?.onDeath?.RemoveListener(OnTargetDeath);
            _currentTarget = null;
            _targetHealth = null;
        }
    }
}