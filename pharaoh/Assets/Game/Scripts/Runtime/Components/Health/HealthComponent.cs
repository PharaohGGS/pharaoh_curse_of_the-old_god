using System;
using System.Collections.Generic;
using System.Linq;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public enum FloatOperation
    {
        Set = 0,
        Increase = 1, 
        Decrease = 2,
    }

    public class HealthComponent : MonoBehaviour
    {
        [Header("Health")] 
        public float startMax = 100;
        public float invincibilityTime = 0f;
        
        public UnityEvent<HealthComponent, float> onHealthSet;
        public UnityEvent<HealthComponent, float> onHealthDecrease;
        public UnityEvent<HealthComponent, float> onHealthIncrease;
        public UnityEvent<HealthComponent> onDeath;
        public UnityEvent<Damager> onTakeHit;

        public bool isInvincible { get; private set; }
        public bool isDead { get; private set; }
        public float max { get; private set; }
        public float current { get; private set; }
        public float percent => current / max;

        private Collider2D[] _colliders;

        [Header("Armors")]
        [SerializeField] private Gear[] gears;
        private readonly List<Gear> _armors = new List<Gear>();
        private float armorDeal
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

        private void ApplyChange(float value, FloatOperation operation)
        {
            switch (operation)
            {
                case FloatOperation.Set:
                    current = value;
                    onHealthSet?.Invoke(this, current);
                    break;
                case FloatOperation.Increase:
                    current = Mathf.Min(current + value, max);
                    onHealthIncrease?.Invoke(this, current);
                    break;
                case FloatOperation.Decrease:
                    current = Mathf.Max(current - value, 0.0f);
                    onHealthDecrease?.Invoke(this, current);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            if (current <= 0.0f)
            {
                isDead = true;
                onDeath?.Invoke(this);
            }
        }
        
        public void Decrease(float value) => ApplyChange(value, FloatOperation.Decrease);
        public void Increase(float value) => ApplyChange(value, FloatOperation.Increase);
        public void Set(float value) => ApplyChange(value, FloatOperation.Set);

        private void Awake()
        {
            isInvincible = false;
            _colliders = GetComponents<Collider2D>();
            if (gears.Length <= 0)
            {
                gears = GetComponentsInChildren<Gear>();
            }
            
            foreach (var gear in gears) if (gear is IArmor) _armors.Add(gear);
        }

        private void Start()
        {
            Set(startMax);
            max = startMax;
        }

        private void OnDisable()
        {
            onHealthSet.RemoveAllListeners();
            onHealthIncrease.RemoveAllListeners();
            onHealthDecrease.RemoveAllListeners();
            onDeath.RemoveAllListeners();
        }

        public void TakeHit(Damager damager, Collider2D other)
        {
            if (isInvincible) return;
            if (!damager || !damager.damagerData) return;
            if (_colliders.Length <= 0 || _colliders.All(col => col != other)) return;

            var damage = damager.damagerData.damage - armorDeal;
            LogHandler.SendMessage($"{name} takes {damage} hit damage from {damager.name.Replace("(Clone)", "")}", MessageType.Log);
            onTakeHit?.Invoke(damager);
            Decrease(damage);

            if (!isInvincible && invincibilityTime > Mathf.Epsilon)
            {
                StartCoroutine(Invincibility());
            }
        }

        private System.Collections.IEnumerator Invincibility()
        {
            isInvincible = true;
            yield return new WaitForSeconds(invincibilityTime);
            isInvincible = false;
        }
    }
}
