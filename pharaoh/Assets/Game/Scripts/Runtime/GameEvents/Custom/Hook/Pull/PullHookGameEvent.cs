using Pharaoh.Gameplay;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Pull Hook GameEvent", menuName = "GameEvents/Custom/Hook/Pull",
        order = 52)]
    public class PullHookGameEvent : HookGameEvent<PullHookCapacity, PullHookBehaviour>
    {
    }
}