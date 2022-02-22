using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class AttackComponent : MonoBehaviour
    {
        public Transform target { get; set; }
        public DamagerHolder holder { get; private set; }
        
        public UnityEvent<Damager> onAttack = new UnityEvent<Damager>();
        public UnityEvent<Damager, Transform> onAimFor = new UnityEvent<Damager, Transform>();

        private void Awake()
        {
            holder = TryGetComponent(out DamagerHolder wh) 
                ? wh : GetComponentInChildren<DamagerHolder>();
        }

        private void OnEnable()
        {
            onAttack?.AddListener(AimTarget);
        }

        private void OnDisable()
        {
            onAttack?.RemoveListener(AimTarget);
        }

        public void Attack() => onAttack?.Invoke(holder.damager);

        public void AimTarget(Damager damager)
        {
            onAimFor?.Invoke(damager, target);

            if (holder.damager is Weapon weapon && weapon.TryGetComponent(out Ballistic ballistic))
            {
                weapon.transform.parent = null;
                weapon.Throw();
            }
        }
    }
}