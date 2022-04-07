using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class DashStunGameEventListener : AbstractGameEventListener<GameObject, StunData, DashStunGameEvent, UnityEvent<GameObject, StunData>> { }
}