using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class DamagerHit2DGameEventListener : AbstractGameEventListener<Damager, Collider2D, DamagerHit2DGameEvent, UnityEvent<Damager, Collider2D>> { }
}