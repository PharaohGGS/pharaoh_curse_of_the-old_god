using Pharaoh.Gameplay;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Hook Grapple GameEvent", menuName = "GameEvents/Custom/Targeting/Grapple", order = 52)]
    public class HookGrappleGameEvent : AbstractGameEvent<HookCapacity, GameObject> { }
}