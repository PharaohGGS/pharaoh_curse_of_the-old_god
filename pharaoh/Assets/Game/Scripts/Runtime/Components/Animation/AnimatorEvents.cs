using System;
using System.Collections.Generic;
using Pharaoh.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Pharaoh.Gameplay.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorEvents : MonoBehaviour
    {
        [SerializeField] private GenericDictionary<string, UnityEvent> events;

        private Animator _animator;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        // TODO : Invoke a method callable via AnimationEvents
        public void CallEvent(string method)
        {
            if (events.TryGetValue(method, out var uEvent))
            {
                uEvent?.Invoke();
            }
        }
    }
}