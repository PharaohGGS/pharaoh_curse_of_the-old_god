using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Damager Hit GameEvent", menuName = "GameEvents/Custom/Damager Hit", order = 52)]
    public class DamagerHitGameEvent : AbstractGameEvent<Damager, Collider2D> { }
}
