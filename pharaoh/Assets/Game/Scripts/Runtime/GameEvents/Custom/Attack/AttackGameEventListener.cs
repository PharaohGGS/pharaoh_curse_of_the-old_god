using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class AttackGameEventListener : AbstractGameEventListener<Gear, GameObject, AttackGameEvent, UnityEvent<Gear, GameObject>> { }
}