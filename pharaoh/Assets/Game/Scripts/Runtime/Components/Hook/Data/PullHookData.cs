using System;
using Pharaoh.Gameplay.Components.Movement;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;

namespace Pharaoh.Gameplay
{
    [CreateAssetMenu(fileName = "New Pull Hook Data", menuName = "HookData/Pull", order = 52)]
    public class PullHookData : ScriptableObject
    {
        public float force = 1f;
        public float maxMovement = 1f;
        public float duration = 1f;
        public float offset = 1f;

        [Tooltip("grapple movement curve from a to b")]
        public AnimationCurve curve;
    }

}
