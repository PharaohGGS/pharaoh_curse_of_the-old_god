using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    public class AimForTargetGameEventListener : AbstractGameEventListener<Gear, Transform, AimForTargetGameEvent, UnityEvent<Gear, Transform>> { }
}
