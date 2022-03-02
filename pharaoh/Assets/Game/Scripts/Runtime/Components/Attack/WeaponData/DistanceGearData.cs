using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "New Distance GearData", menuName = "Gears/", order = 0)]
    public class DistanceGearData : GearData
    {
        public float rate;
        public float range;

    }
}