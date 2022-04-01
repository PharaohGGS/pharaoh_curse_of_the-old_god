using UnityEngine;

namespace Pharaoh.Gameplay
{
    [CreateAssetMenu(fileName = "New Melee Trap Data", menuName = "Data/Trap/Melee")]
    public class MeleeTrapData : TrapData
    {
        public float showingSpeed;
        public float hidingSpeed;
        public AnimationCurve curve;
    }
}