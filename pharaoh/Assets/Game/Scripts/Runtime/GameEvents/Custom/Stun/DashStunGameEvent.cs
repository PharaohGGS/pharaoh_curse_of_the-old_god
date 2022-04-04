using Pharaoh.Gameplay;
using Pharaoh.Gameplay.Components;
using UnityEngine;

namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Dash Stun GameEvent", menuName = "GameEvents/Custom/Dash/Stun", order = 52)]
    public class DashStunGameEvent : AbstractGameEvent<GameObject, StunData> { }
}