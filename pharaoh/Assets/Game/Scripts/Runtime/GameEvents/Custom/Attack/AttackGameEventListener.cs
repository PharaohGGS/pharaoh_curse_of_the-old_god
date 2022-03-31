using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class AttackGameEventListener : AbstractGameEventListener<int, GameObject, AttackGameEvent, UnityEvent<int, GameObject>> { }
}