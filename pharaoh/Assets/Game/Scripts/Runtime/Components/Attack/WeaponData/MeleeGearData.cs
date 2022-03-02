using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "New Melee GearData", menuName = "Gears/", order = 0)]
    public class MeleeGearData : GearData
    {
        public float rate;
        public float range;

        public bool throwable;
        [HideInInspector] public float throwableRange;
        [HideInInspector] public float throwablePickingTime = 1f;
    }
}