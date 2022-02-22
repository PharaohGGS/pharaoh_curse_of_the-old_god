using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class AttackComponent : MonoBehaviour
    {
        public Transform target { get; set; }
        public WeaponHolder holder { get; private set; }

        public UnityEvent<Weapon, Transform> onAimFor;

        private void Awake()
        {
            holder = TryGetComponent(out WeaponHolder wh) 
                ? wh : GetComponentInChildren<WeaponHolder>();
        }

        public void Attack()
        {
            if (holder.weapon.TryGetComponent(out Ballistic ballistic))
            {
                onAimFor?.Invoke(holder.weapon, target);

                holder.weapon.attach = null;
                holder.weapon.onThrown?.Invoke();
            }
        }
    }
}