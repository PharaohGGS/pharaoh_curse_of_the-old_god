using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class AttackComponent : MonoBehaviour
    {
        public Transform target { get; set; }
        [field: SerializeField] public DamagerHolder[] holders { get; private set; }
        public Dictionary<DamagerData, DamagerHolder> dataHolders { get; private set; }

        public UnityEvent<Damager> onDamagerAttack = new UnityEvent<Damager>();
        public UnityEvent<Damager, Transform> onDamagerAimTarget = new UnityEvent<Damager, Transform>();

        private void Awake()
        {
            if (holders.Length <= 0)
            {
                holders = GetComponentsInChildren<DamagerHolder>();
            }

            dataHolders = new Dictionary<DamagerData, DamagerHolder>();
            foreach (var holder in holders)
            {
                dataHolders.TryAdd(holder.data, holder);
            }
        }

        public void Attack(DamagerData data)
        {
            var damager = dataHolders[data]?.damager;
            if (damager == null) return;
            
            onDamagerAimTarget?.Invoke(damager, target);
            onDamagerAttack?.Invoke(damager);
        }
    }
}