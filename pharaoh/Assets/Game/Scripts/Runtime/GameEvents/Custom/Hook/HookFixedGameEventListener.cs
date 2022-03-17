using Pharaoh.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class HookFixedGameEventListener : AbstractGameEventListener<HookTargeting, GameObject, HookFixedGameEvent, UnityEvent<HookTargeting, GameObject>> { }
}