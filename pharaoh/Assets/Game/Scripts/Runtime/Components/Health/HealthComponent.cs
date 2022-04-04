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

        public UnityEvent<HealthComponent, float> onHealthChange;
        public UnityEvent<HealthComponent> onDeath;

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
            current = operation switch
            {
                FloatOperation.Set => Mathf.Min(value, max),
                FloatOperation.Increase => Mathf.Min(current + value, max),
                FloatOperation.Decrease => Mathf.Max(current - value, 0.0f),
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };

            onHealthChange?.Invoke(this, current);

            if (current <= 0.0f)
            {
                onDeath?.Invoke(this);
            } 
        }
        
        public void Decrease(float value) => ApplyChange(value, FloatOperation.Decrease);
        public void Increase(float value) => ApplyChange(value, FloatOperation.Increase);
        public void Set(float value) => ApplyChange(value, FloatOperation.Set);

        private void Awake()
        {
            _colliders = GetComponents<Collider2D>();
            if (gears.Length <= 0)
            {
                gears = GetComponentsInChildren<Gear>();
            }
            
            foreach (var gear in gears) if (gear is IArmor) _armors.Add(gear);
        }

        private void Start()
        {
            max = startMax;
            Set(startMax);
        }

        private void OnDisable()
        {
            onHealthChange.RemoveAllListeners();
            onDeath.RemoveAllListeners();
        }

        public void TakeHit(Damager damager, Collider2D other)
        {
            if (!damager || !damager.damagerData) return;
            if (_colliders.Length <= 0 || _colliders.All(col => col != other)) return;

            var damage = damager.damagerData.damage - armorDeal;

            LogHandler.SendMessage($"{name} takes {damage} hit damage from {damager.name.Replace("(Clone)", "")}", MessageType.Log);
            Decrease(damage);
        }
    }
}
