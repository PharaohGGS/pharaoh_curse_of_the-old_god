using Pharaoh.Gameplay;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Grab Hook GameEvent", menuName = "GameEvents/Custom/Hook/Grab",
        order = 52)]
    public class GrabHookGameEvent : HookGameEvent<GrabHookCapacity, GrabHookBehaviour>
    {
    }
}