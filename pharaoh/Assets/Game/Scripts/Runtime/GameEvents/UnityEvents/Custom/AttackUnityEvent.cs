using System;
using Pharaoh.Gameplay.Components;
using UnityEngine.Events;

namespace Pharaoh.GameEvents
{
    [Serializable] public class AttackUnityEvent : UnityEvent<Gear> { }
}