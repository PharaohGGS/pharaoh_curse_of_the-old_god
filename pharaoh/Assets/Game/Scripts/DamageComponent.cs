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
        [HideInInspector] public DamageDealingType damageDealingType;

        [Header("FireRate")] 
        
        [HideInInspector] public float fireRate;
        [HideInInspector] public float damagePerRate;

        [Header("OneHit")] 
        
        [HideInInspector] public float damagePerHit;
    }
}
