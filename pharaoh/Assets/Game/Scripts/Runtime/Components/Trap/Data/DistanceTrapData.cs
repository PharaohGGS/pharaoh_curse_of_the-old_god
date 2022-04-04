using UnityEngine;

namespace Pharaoh.Gameplay
{
    [CreateAssetMenu(fileName = "New Distance Trap Data", menuName = "Data/Trap/Distance")]
    public class DistanceTrapData : TrapData
    {
        public float initialVelocity;
    }
}