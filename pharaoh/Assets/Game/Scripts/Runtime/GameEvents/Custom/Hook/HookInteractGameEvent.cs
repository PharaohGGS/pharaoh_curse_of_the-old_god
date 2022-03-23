using Pharaoh.Gameplay;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Hook Interact GameEvent", menuName = "GameEvents/Custom/Targeting/Interact", order = 52)]
    public class HookInteractGameEvent : AbstractGameEvent<HookCapacity, GameObject> { }
}