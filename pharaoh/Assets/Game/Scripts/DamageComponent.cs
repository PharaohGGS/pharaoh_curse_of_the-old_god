using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Gameplay.Component
{
    public enum DamageDealingType
    {
        FireRate,
        OneHit,
    }

    public class DamageComponent : MonoBehaviour
    {
        public delegate void DApplyDamage(GameObject objectHit, float damage);
        public static event DApplyDamage OnApplyDamage;

        [SerializeField] private DamageDealingType damageDealingType;

        [Header("FireRate")] 
        
        [SerializeField] private float fireRate;
        [SerializeField] private float damagePerRate;

        [Header("OneHit")] 
        
        [SerializeField] private float damagePerHit;

        public void Hit(GameObject objectToHit)
        {
            switch (damageDealingType)
            {
                case DamageDealingType.FireRate:
                    OnApplyDamage?.Invoke(objectToHit, damagePerRate);
                    break;
                case DamageDealingType.OneHit:
                    OnApplyDamage?.Invoke(objectToHit, damagePerHit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
