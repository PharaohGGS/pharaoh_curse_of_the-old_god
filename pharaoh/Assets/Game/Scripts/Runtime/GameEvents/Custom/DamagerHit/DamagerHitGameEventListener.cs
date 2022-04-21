using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class DamagerHitGameEventListener : AbstractGameEventListener<Damager, Collision2D, DamagerHitGameEvent, UnityEvent<Damager, Collision2D>> { }
}