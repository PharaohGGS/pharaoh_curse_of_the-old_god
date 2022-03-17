using Pharaoh.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class HookBestTargetSelectedGameEventListener : AbstractGameEventListener<HookTargeting, GameObject, HookBestTargetSelectedGameEvent, UnityEvent<HookTargeting, GameObject>> { }
}