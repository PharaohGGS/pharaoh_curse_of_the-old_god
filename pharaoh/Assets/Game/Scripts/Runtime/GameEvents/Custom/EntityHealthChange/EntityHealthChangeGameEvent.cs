using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Entity Health Change GameEvent", menuName = "GameEvents/Custom/Entity Health Change",
        order = 52)]
    public class EntityHealthChangeGameEvent : AbstractGameEvent<HealthComponent, float>
    {
    }
}