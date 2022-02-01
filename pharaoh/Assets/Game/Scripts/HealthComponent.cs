using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Gameplay.Component
{
    public enum HealthOperation
    {
        Define = 0,
        Increase = 1, 
        Decrease = 2,
    }

    public class HealthComponent : MonoBehaviour
    {
        public delegate void DHealthUpdate(HealthComponent healthComponent, float healthUpdate);
        public event DHealthUpdate OnHealthUpdate;
        
        [field: SerializeField] public float MaxHealth { get; private set; }

        private float _currentHealth;
        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = Mathf.Max(0, Mathf.Min(value, MaxHealth));
                OnHealthUpdate?.Invoke(this, _currentHealth);
            }
        }

        public float CurrentPercentHealth => CurrentHealth / MaxHealth;

        public float UpdateHealth(float val, HealthOperation operation)
        {
            switch (operation)
            {
                case HealthOperation.Increase:
                    CurrentHealth += val;
                    break;
                case HealthOperation.Decrease:
                    CurrentHealth -= val;
                    break;
                case HealthOperation.Define:
                    CurrentHealth = MaxHealth = val;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            return CurrentHealth;
        }

        public bool TryUpdateCurrency(float val, HealthOperation operation, out float updated)
        {
            var beforeUpdate = CurrentHealth;
            updated = UpdateHealth(val, operation);
            return Mathf.Abs(beforeUpdate - updated) > Mathf.Epsilon;
        }

        private void Start()
        {
            CurrentHealth = MaxHealth;
        }
    }
}
