using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class DamagerHitGameEventListener : AbstractGameEventListener<Damager, Collider2D, DamagerHitGameEvent, UnityEvent<Damager, Collider2D>> { }
}