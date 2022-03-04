using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Aim For Target GameEvent", menuName = "GameEvents/Custom/Aim For Target",
        order = 52)]
    public class AimForTargetGameEvent : AbstractGameEvent<Transform>
    {
    }
}