using Pharaoh.Gameplay.Components;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class AttackGameEventListener : AbstractGameEventListener<Gear, AttackGameEvent, UnityEvent<Gear>> { }
}