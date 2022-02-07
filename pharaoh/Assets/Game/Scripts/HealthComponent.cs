using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pharaoh.Tools;

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

        public delegate void DHealthUnderZero(HealthComponent healthComponent);
        public event DHealthUnderZero OnHealthUnderZero;
        
        [field: SerializeField] public float MaxHealth { get; private set; }

        private float _currentHealth;
        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = Mathf.Max(0, Mathf.Min(value, MaxHealth));
                OnHealthUpdate?.Invoke(this, _currentHealth);

                if (_currentHealth <= 0.0f)
                {
                    OnHealthUnderZero?.Invoke(this);
                }
            }
        }

        public float CurrentHealthPercent => CurrentHealth / MaxHealth;

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

        public bool TryUpdateHealth(float val, HealthOperation operation, out float updated)
        {
            var beforeUpdate = CurrentHealth;
            updated = UpdateHealth(val, operation);
            return Mathf.Abs(beforeUpdate - updated) > Mathf.Epsilon;
        }

        private void ReceiveDamage(GameObject objectHit, float damage)
        {
            if (!this.IsSharingSameInstance(objectHit))
            {
                LogHandler.SendMessage("[HealthComponent] Not sharing the same gameobject as damage receiver.", MessageType.Error);
                return;
            }

            if (!TryUpdateHealth(damage, HealthOperation.Decrease, out float updated))
            {
                LogHandler.SendMessage($"[HealthComponent] Health hasn't been updated, current: {updated}", MessageType.Warning);
                return;
            }
            
            LogHandler.SendMessage($"{gameObject.name} health: {updated}", MessageType.Log);
        }

        private void Start()
        {
            CurrentHealth = MaxHealth;
        }

        private void OnEnable()
        {
            DamageComponent.OnApplyDamage += ReceiveDamage;
        }

        private void OnDisable()
        {
            DamageComponent.OnApplyDamage -= ReceiveDamage;
        }
    }
}
