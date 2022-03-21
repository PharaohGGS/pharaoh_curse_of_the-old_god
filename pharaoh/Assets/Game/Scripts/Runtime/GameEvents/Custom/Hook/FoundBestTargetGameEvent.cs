using Pharaoh.Gameplay;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New FoundBestTarget GameEvent", menuName = "GameEvents/Custom/Targeting/FoundBestTarget",
        order = 52)]
    public class FoundBestTargetGameEvent : AbstractGameEvent<TargetFinder, GameObject>
    {
    }
}