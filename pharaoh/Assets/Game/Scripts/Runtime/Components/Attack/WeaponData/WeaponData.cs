using System.Collections;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "New WeaponData", menuName = "Weapons/", order = 0)]
    public abstract class WeaponData : ScriptableObject
    {
        public string name;
        public bool canThrow;
        public float damage;
        public float attackRate;
        public GameObject prefab;
    }
}