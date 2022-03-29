using Pharaoh.Gameplay;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New FoundBestTarget GameEvent", menuName = "GameEvents/Custom/Targeting/FoundBestTarget",
        order = 52)]
    public class HookFoundTargetGameEvent : AbstractGameEvent<HookCapacity, GameObject>
    {
    }
}