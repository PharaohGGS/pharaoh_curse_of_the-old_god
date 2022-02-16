using UnityEngine;

namespace Pharaoh.Gameplay.Components.Events
{
    [CreateAssetMenu(fileName = "New Void GameEvent", menuName = "GameEvents/Engine/Void", order = 52)]
    public class VoidGameEvent : AbstractGameEvent<Events.Void>
    {
        public void Raise() => Raise(new Events.Void());
    }
}