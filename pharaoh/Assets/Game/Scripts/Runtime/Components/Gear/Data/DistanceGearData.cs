using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "GearData_Distance", menuName = "Gears/Distance", order = 54)]
    public class DistanceGearData : GearData
    {
        [Tooltip("in m/s")] public float shootInitialVelocity;
    }
}