using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public abstract class GearData : ScriptableObject
    {
        public string description;

        [Header("Attack")] public bool canAttack;

        [HideInInspector] public float rate;
        [HideInInspector] public float range;
    }
}