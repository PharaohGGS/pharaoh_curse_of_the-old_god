using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Void GameEvent", menuName = "GameEvents/Engine/Void", order = 52)]
    public class VoidGameEvent : AbstractGameEvent<Void>
    {
        public void Raise() => Raise(new Void());
    }
}