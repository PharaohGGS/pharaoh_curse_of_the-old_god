using System;
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
        [field: SerializeField] public float max { get; private set; } = 100;

        public UnityEvent<float> OnHealthChange;
        public UnityEvent OnDeath;

        public float current { get; private set; }
        public float percent => current / max;

        private void ApplyChange(float value, FloatOperation operation)
        {
            current = operation switch
            {
                FloatOperation.Set => Mathf.Min(value, max),
                FloatOperation.Increase => Mathf.Min(current + value, max),
                FloatOperation.Decrease => Mathf.Max(current - value, 0.0f),
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };

            OnHealthChange?.Invoke(current);

            if (current <= 0.0f)
            {
                OnDeath?.Invoke();
            } 
        }
        
        public void Decrease(float value) => ApplyChange(value, FloatOperation.Decrease);
        public void Increase(float value) => ApplyChange(value, FloatOperation.Increase);
        public void Set(float value) => ApplyChange(value, FloatOperation.Set);
        
        private void Start()
        {
            current = max;
        }

        private void OnDisable()
        {
            OnHealthChange.RemoveAllListeners();
            OnDeath.RemoveAllListeners();
        }

        public void TakeHit(Damager damager, Collider2D other)
        {
            LogHandler.SendMessage($"HealthComponent: {name}, Damager: {damager.name}, other: {other.name}", MessageType.Log);
            var colliders = GetComponents<Collider2D>();
            if (colliders.Length <= 0 || colliders.All(col => col != other)) return;

            var damage = damager.data.damage;
            LogHandler.SendMessage($"{name} takes {damage} hit damage from {damager.name.Replace("(Clone)", "")}", MessageType.Log);
            Decrease(damage);
        }
    }
}
