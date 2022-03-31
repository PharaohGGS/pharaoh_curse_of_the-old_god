using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Entity Died GameEvent", menuName = "GameEvents/Custom/Entity Died",
        order = 52)]
    public class EntityDiedGameEvent : AbstractGameEvent<HealthComponent>
    {
    }
}