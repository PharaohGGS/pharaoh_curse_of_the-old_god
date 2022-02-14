using System;
using Pharaoh.Gameplay.Components.Events;
using UnityEngine;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public enum FloatOperation
    {
        Define = 0,
        Increase = 1, 
        Decrease = 2,
    }

    public class HealthComponent : MonoBehaviour
    {
        [field: SerializeField] public float maxHealth { get; private set; } = 100;
        [field: SerializeField] public float health { get; private set; } = 100;

        public UnityEvent<float> OnHealthChange;
        public UnityEvent OnDeath;

        public float Percent => health / maxHealth;

        public void ApplyChange(float val, FloatOperation operation)
        {
            health = operation switch
            {
                FloatOperation.Define => Mathf.Min(val, maxHealth),
                FloatOperation.Increase => Mathf.Min(health + val, maxHealth),
                FloatOperation.Decrease => Mathf.Max(health - val, 0.0f),
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };

            OnHealthChange?.Invoke(health);

            if (health <= 0.0f)
            {
                OnDeath?.Invoke();
            } 
        }

        private void Start()
        {
            health = maxHealth;
        }

        private void OnDisable()
        {
            OnHealthChange.RemoveAllListeners();
        }
    }
}
