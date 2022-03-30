using Pharaoh.Gameplay;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Trap Start GameEvent", menuName = "GameEvents/Custom/Trap/Start",
        order = 52)]
    public class TrapStartGameEvent : AbstractGameEvent<TrapCapacity, TrapBehaviour>
    {
    }
}