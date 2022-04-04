using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [CreateAssetMenu(fileName = "New Stun Data", menuName = "Data/Stun")]
    public class StunData : ScriptableObject
    {
        public string type;
        public float time;
    }
}