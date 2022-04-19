using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class BlinkComponent : MonoBehaviour
    {
        [Header("Meshes")] public Renderer[] renderers;

        [Header("Feedbacks")]
        [SerializeField] private float duration;
        [SerializeField] private int blinkCount;
        [SerializeField] private float intensity;
        [SerializeField, ColorUsage(false, false)] 
        private Color color = Color.white;

        private float _timer;
        private float _pingpongTimer;

        private readonly List<Color[]> _materialsColors = new List<Color[]>();

        private void Awake()
        {
            // copy base color in list
            foreach (var r in renderers)
            {
                var size = r.materials.Length;
                var colors = new Color[size];
                for (var i = 0; i < size; i++) colors[i] = r.materials[i].color;
                _materialsColors.Add(colors);
            }
        }

        public void Blink()
        {
            _timer = duration;
            _pingpongTimer = duration / blinkCount;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            _pingpongTimer -= Time.deltaTime;
            float lerp = Mathf.Clamp01(_pingpongTimer / (duration / blinkCount));
            float lerpIntensity = (lerp * intensity) + 1.0f;

            for (var rIndex = 0; rIndex < renderers.Length; rIndex++)
            {
                var r = renderers[rIndex];
                var colors = _materialsColors[rIndex];

                for (var mIndex = 0; mIndex < r.materials.Length; mIndex++)
                {
                    var material = r.materials[mIndex];
                    material.color = _timer <= 0.0f ? colors[mIndex] : color * lerpIntensity;
                }
            }

            if (_pingpongTimer <= 0.0f && _timer > 0.0f)
            {
                _pingpongTimer = duration / blinkCount;
            }
        }
    }
}