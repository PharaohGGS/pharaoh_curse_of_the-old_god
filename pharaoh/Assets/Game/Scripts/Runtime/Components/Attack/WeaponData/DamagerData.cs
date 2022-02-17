using System.Collections;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "New DamagerData", menuName = "Weapons/", order = 0)]
    public abstract class DamagerData : ScriptableObject
    {
        public string description;
        public bool canThrow;
        public float damage;
        public float attackRate;
        public float attackRange;
        public GameObject prefab;
    }
}