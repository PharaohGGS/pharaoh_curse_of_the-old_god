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
        [field: SerializeField] public float max { get; private set; } = 100;

        public UnityEvent<float> OnHealthChange;
        public UnityEvent OnDeath;

        public float current { get; private set; }
        public float percent => current / max;

        public void ApplyChange(float val, FloatOperation operation)
        {
            current = operation switch
            {
                FloatOperation.Define => Mathf.Min(val, max),
                FloatOperation.Increase => Mathf.Min(current + val, max),
                FloatOperation.Decrease => Mathf.Max(current - val, 0.0f),
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };

            OnHealthChange?.Invoke(current);

            if (current <= 0.0f)
            {
                OnDeath?.Invoke();
            } 
        }

        private void Start()
        {
            current = max;
        }

        private void OnDisable()
        {
            OnHealthChange.RemoveAllListeners();
        }
    }
}
