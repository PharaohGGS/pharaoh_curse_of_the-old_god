using Pharaoh.Gameplay.Components;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class EntityHealthChangeGameEventListener : AbstractGameEventListener<HealthComponent, float, EntityHealthChangeGameEvent, UnityEvent<HealthComponent, float>> { }
}