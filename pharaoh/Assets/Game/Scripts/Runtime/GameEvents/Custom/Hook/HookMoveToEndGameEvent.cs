using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Hook MoveToEnd GameEvent", menuName = "GameEvents/Custom/Hook/MoveToEnd",
        order = 52)]
    public class HookMoveToEndGameEvent : AbstractGameEvent<Pharaoh.Gameplay.HookTargeting, GameObject>
    {
    }
}