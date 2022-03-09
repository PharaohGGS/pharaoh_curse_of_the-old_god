using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New DamagerShot GameEvent", menuName = "GameEvents/Custom/DamagerShot",
        order = 55)]
    public class DamagerShotGameEvent : AbstractGameEvent<Damager>
    {
    }
}