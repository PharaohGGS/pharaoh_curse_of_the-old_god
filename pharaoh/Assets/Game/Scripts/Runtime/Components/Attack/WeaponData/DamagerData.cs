using System.Collections;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "New DamagerData", menuName = "Weapons/", order = 0)]
    public abstract class DamagerData : ScriptableObject
    {
        public Damager prefab;
        public string description;
        public float damage;
    }
}