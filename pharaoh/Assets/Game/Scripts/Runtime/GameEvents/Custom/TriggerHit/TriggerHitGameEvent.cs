using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Trigger Hit GameEvent", menuName = "GameEvents/Custom/Trigger Hit", order = 52)]
    public class TriggerHitGameEvent : AbstractGameEvent<Damager> { }
}
