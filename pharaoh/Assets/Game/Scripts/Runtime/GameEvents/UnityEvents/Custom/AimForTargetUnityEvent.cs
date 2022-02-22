using System;
using Pharaoh.Gameplay.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    [Serializable] public class AimForTargetUnityEvent : UnityEvent<Damager, Transform> { }
}