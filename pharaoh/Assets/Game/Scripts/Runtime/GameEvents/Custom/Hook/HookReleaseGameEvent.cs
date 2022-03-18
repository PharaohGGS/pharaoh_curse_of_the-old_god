using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Hook Release GameEvent", menuName = "GameEvents/Custom/Hook/Release",
        order = 52)]
    public class HookReleaseGameEvent : AbstractGameEvent<Pharaoh.Gameplay.HookCapacity>
    {
    }
}