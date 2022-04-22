using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Pharaoh.Gameplay
{
    [System.Serializable]
    public enum FadeTransition
    {
        In,
        Out,
    }

    [RequireComponent(typeof(VisualEffect))]
    public class HookIndicatorVFX : MonoBehaviour
    {
        private VisualEffect _vfx;
        [SerializeField] private List<Fade> properties;
        [SerializeField, Header("Hook Behaviour Events")] private HookBehaviourEvents events;

        private Coroutine _coroutine;
        private FadeTransition _lastTransition;

        private void Awake()
        {
            _vfx = GetComponent<VisualEffect>();
        }
        
        private void OnEnable()
        {
            // Hook bindings
            if (events) events.started += OnHookStarted;
        }

        private void OnDisable()
        {
            // Hook bindings
            if (events) events.started -= OnHookStarted;
        }

        private void OnHookStarted(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;
            FadeOut();
        }

        public void FadeIn()
        {
            if (_lastTransition == FadeTransition.In) return;
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(FloatFading(FadeTransition.In));
        }

        public void FadeOut()
        {
            if (_lastTransition == FadeTransition.Out) return;
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(FloatFading(FadeTransition.Out));
        }

        private IEnumerator FloatFading(FadeTransition transition)
        {
            Debug.Log($"{name} is Fading {transition}");
            _lastTransition = transition;
            bool[] ended = new bool[properties.Count];

            float target = 0.0f;
            float maxDelta = 0.0f;
            float lerp = 0.0f;
            
            while (true)
            {
                for (var i = 0; i < properties.Count; i++)
                {
                    var property = properties[i];
                    if (ended[i]) continue;

                    property.DoFading(_vfx, transition);            
                }
                
                if (Array.TrueForAll(ended, b => b == true)) break;
                yield return null;
            }
        }
    }
}