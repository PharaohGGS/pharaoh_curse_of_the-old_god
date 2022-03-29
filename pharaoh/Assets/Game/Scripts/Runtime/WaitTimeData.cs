using UnityEngine;

namespace Pharaoh.Gameplay
{
    [CreateAssetMenu(fileName = "New WaitTime Data", menuName = "Data/WaitTime", order = 0)]
    public class WaitTimeData : ScriptableObject
    {
        public float seconds;
    }
}