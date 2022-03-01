using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Attack GameEvent", menuName = "GameEvents/Custom/Attack",
        order = 52)]
    public class AttackGameEvent : AbstractGameEvent<Damager>
    {
    }
}