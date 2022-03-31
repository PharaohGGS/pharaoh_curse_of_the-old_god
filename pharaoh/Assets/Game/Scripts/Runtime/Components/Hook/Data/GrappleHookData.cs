using UnityEngine;

namespace Pharaoh.Gameplay
{
    [CreateAssetMenu(fileName = "New Grapple Hook Data", menuName = "HookData/Grapple", order = 52)]
    public class GrappleHookData : ScriptableObject
    {
        [Tooltip("grapple speed toward target in m/s")]
        public float speed;
        [Tooltip("grapple movement curve from a to b")]
        public AnimationCurve curve;
    }
}