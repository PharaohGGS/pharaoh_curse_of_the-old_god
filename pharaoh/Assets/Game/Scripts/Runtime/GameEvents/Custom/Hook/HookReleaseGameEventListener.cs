using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class HookReleaseGameEventListener : AbstractGameEventListener<Transform, Transform, HookGrabGameEvent, UnityEvent<Transform, Transform>> { }
}