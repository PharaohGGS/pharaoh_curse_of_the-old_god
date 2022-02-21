using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class AttackComponent : MonoBehaviour
    {
        private Transform _target;
        public Transform target
        {
            get => _target;
            set
            {
                _target = value;
                OnAimFor?.Invoke(target);
            }
        }

        public WeaponHolder holder { get; private set; }

        public UnityEvent<Transform> OnAimFor;

        private void Awake()
        {
            holder = TryGetComponent(out WeaponHolder wh) 
                ? wh : GetComponentInChildren<WeaponHolder>();
        }

        public void Attack()
        {
            if (holder.weapon.TryGetComponent(out Ballistic ballistic))
            {
                holder.weapon.attach = null;
                holder.weapon.onThrown?.Invoke();
            }
        }
    }
}