using Pharaoh.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class HookReleasedGameEventListener : AbstractGameEventListener<HookTargeting, GameObject, HookReleasedGameEvent, UnityEvent<HookTargeting, GameObject>> { }
}