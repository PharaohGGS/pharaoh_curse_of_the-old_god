using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Hook Fix GameEvent", menuName = "GameEvents/Custom/Hook/Fix",
        order = 52)]
    public class HookFixedGameEvent : AbstractGameEvent<Pharaoh.Gameplay.HookTargeting, GameObject>
    {
    }
}
