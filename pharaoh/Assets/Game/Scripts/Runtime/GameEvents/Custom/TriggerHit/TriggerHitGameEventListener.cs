using Pharaoh.Gameplay.Components;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class TriggerHitGameEventListener : AbstractGameEventListener<Damager, TriggerHitGameEvent, UnityEvent<Damager>> { }
}