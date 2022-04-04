using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Distance Gear Shoot GameEvent", menuName = "GameEvents/Custom/Distance Gear Shoot",
        order = 55)]
    public class DistanceGearShootGameEvent : AbstractGameEvent<Damager>
    {
    }
}