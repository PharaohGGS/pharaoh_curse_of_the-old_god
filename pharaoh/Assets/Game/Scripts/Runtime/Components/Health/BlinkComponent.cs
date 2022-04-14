using System;
using System.Collections;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class BlinkComponent : MonoBehaviour
    {
        [Header("Meshes")] public Renderer[] renderers;

        [Header("Feedbacks")]
        [SerializeField] 
        private float duration;
        [SerializeField] 
        private float intensity;
        [SerializeField, ColorUsage(false, false)] 
        private Color color = Color.white;

        private float _timer;

        private Color[] _colors;

        private void Awake()
        {
            _colors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                _colors[i] = renderers[i].material.color;
            }
        }

        public void Blink()
        {
            _timer = duration;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            float lerp = Mathf.Clamp01(_timer / duration);
            float lerpIntensity = (lerp * intensity) + 1.0f;

            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = _timer <= 0.0f ? _colors[i] : color * lerpIntensity;
            }
        }
    }
}