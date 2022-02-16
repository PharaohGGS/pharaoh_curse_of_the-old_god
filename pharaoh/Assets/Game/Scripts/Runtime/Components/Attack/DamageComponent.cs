using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class DamageComponent : MonoBehaviour
    {
        public UnityEvent<HealthComponent> OnHit;

        public void ApplyDamage(HealthComponent health)
        {
            OnHit?.Invoke(health);
        }
    }
}
