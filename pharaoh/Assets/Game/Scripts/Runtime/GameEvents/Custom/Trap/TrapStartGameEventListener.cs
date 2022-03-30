using Pharaoh.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class TrapStartGameEventListener : AbstractGameEventListener<TrapCapacity, TrapBehaviour, TrapStartGameEvent, UnityEvent<TrapCapacity, TrapBehaviour>> { }
}