using System;
using Pharaoh.Gameplay.Components;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Tools.Debug;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using MessageType = Pharaoh.Tools.Debug.MessageType;
using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;

namespace Pharaoh.Gameplay
{
    [CreateAssetMenu(fileName = "New Snatch Hook Data", menuName = "HookData/Snatch", order = 52)]
    public class SnatchHookData : ScriptableObject
    {
        public float offset;
        public float force;
        public AnimationCurve curve;
    }
}