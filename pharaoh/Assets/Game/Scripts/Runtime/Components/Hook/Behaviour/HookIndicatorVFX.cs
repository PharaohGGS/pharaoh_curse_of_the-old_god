using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(VisualEffect))]
    public class HookIndicatorVFX : MonoBehaviour
    {
        private VisualEffect _vfx;
        [SerializeField] private Fade<float>[] floatProperties;
        [SerializeField, Header("Hook Behaviour Events")] private HookBehaviourEvents events;

        [System.Serializable]
        private enum FadeTransition
        {
            In,
            Out,
        }
        
        [System.Serializable]
        private class Fade<T>
        {
            public string name;
            
            public T max;
            public T min;
            
            public float speedIn;
            public float speedOut;
        }

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
            bool[] ended = new bool[floatProperties.Length];

            float maxDelta = 0.0f;
            float current = 0.0f;
            float lerp = 0.0f;
            
            while (true)
            {
                for (var i = 0; i < floatProperties.Length; i++)
                {
                    var property = floatProperties[i];
                    if (ended[i]) continue;

                    switch (transition)
                    {
                        case FadeTransition.In:
                            maxDelta = Time.deltaTime * property.speedIn;
                            current = _vfx.GetFloat(property.name);
                            ended[i] = Mathf.Abs(current - property.max) <= Mathf.Epsilon;
                            lerp = maxDelta <= Mathf.Epsilon ? property.max : Mathf.MoveTowards(current, property.max, maxDelta);
                            if (!ended[i]) _vfx.SetFloat(property.name, lerp);
                            break;
                        case FadeTransition.Out:
                            maxDelta = Time.deltaTime * property.speedOut;
                            current = _vfx.GetFloat(property.name);
                            ended[i] = Mathf.Abs(current - property.min) <= Mathf.Epsilon;
                            lerp = maxDelta <= Mathf.Epsilon ? property.min : Mathf.MoveTowards(current, property.min, maxDelta);
                            if (!ended[i]) _vfx.SetFloat(property.name, lerp);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(transition), transition, null);
                    }               
                }
                
                if (Array.TrueForAll(ended, b => b == true)) break;
                yield return null;
            }
        }
    }
}