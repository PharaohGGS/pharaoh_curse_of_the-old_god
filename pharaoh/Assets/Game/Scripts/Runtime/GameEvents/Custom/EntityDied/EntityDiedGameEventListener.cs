using Pharaoh.Gameplay.Components;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class EntityDiedGameEventListener : AbstractGameEventListener<HealthComponent, EntityDiedGameEvent, UnityEvent<HealthComponent>> { }
}