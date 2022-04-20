using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Shake GameEvent", menuName = "GameEvents/Custom/Shake", order = 52)]
    public class ShakeGameEvent : AbstractGameEvent<float, float, float> { }
}