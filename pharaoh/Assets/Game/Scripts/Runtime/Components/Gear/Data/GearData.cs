using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public enum GearType
    {
        Null = -1,
        Defense = 0,
        Distance = 1,
        Melee = 2,
    }

    public abstract class GearData : ScriptableObject
    {
        public string description;

        [Header("Attack")] public bool canAttack;

        [HideInInspector] public float delay;
        [HideInInspector] public float rate;
        [HideInInspector] public float range;

        public abstract GearType GetGearType();
    }
}