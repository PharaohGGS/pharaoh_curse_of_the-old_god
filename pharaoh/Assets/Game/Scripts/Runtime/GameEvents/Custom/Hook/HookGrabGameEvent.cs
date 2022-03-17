using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pharaoh.GameEvents
{
    [CreateAssetMenu(fileName = "New Hook Grab GameEvent", menuName = "GameEvents/Custom/Hook/Grab",
        order = 52)]
    public class HookGrabGameEvent : AbstractGameEvent<Pharaoh.Gameplay.HookTargeting, Transform>
    {
    }
}
