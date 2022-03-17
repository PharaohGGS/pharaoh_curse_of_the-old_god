using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class HookGrabGameEventListener : AbstractGameEventListener<Transform, Transform, HookGrabGameEvent, UnityEvent<Transform, Transform>> { }
}