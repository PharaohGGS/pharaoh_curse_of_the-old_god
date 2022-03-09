using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "GearData_Melee", menuName = "Gears/Melee", order = 54)]
    public class MeleeGearData : GearData
    {
        public bool throwable;
        [HideInInspector] public float throwableRange;
        [HideInInspector] public float throwablePickingTime = 1f;
        [HideInInspector] public float throwableInitialVelocity = 5f;

        public override GearType GetGearType() => GearType.Melee;
    }
}