using System;
using Pharaoh.Gameplay.Components;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    [Serializable] public class TriggerHitUnityEvent : UnityEvent<Damager> { }
}