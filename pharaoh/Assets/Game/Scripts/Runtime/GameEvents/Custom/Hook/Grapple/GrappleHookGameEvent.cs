using Pharaoh.Gameplay;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Grapple Hook GameEvent", menuName = "GameEvents/Custom/Hook/Grapple",
        order = 52)]
    public class GrappleHookGameEvent : HookGameEvent<GrappleHookCapacity, GrappleHookBehaviour>
    {
    }
}