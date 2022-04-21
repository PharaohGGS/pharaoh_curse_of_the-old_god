using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Gameplay.Components.Movement;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class HitEffectBehaviour : MonoBehaviour
    {
        [Header("Blink")]
        [HideInInspector, SerializeField] private bool useBlink;
        [HideInInspector, SerializeField] private Renderer[] blkRenderers;
        [HideInInspector, SerializeField, Range(0.0f, 10.0f)] private float blkDuration;
        [HideInInspector, SerializeField, Range(1, 100)] private int blkCount;
        [HideInInspector, SerializeField, Range(0.0f, 100.0f)] private float blkIntensity;
        [HideInInspector, SerializeField, ColorUsage(false, false)] private Color blkColor = Color.white;
        private readonly List<Color[]> _blkMaterialsColors = new List<Color[]>();

        [Header("Knockback")]
        [HideInInspector, SerializeField] private bool useKnockBack;
        [HideInInspector, SerializeField, Range(0.0f, 100.0f)] private float kbForce;
        [HideInInspector, SerializeField, Range(0.0f, 10.0f)] private float kbDuration;
        [HideInInspector, SerializeField] private UnityEvent onKnockBackStart;
        [HideInInspector, SerializeField] private UnityEvent onKnockBackEnd;

        [Header("ScreenShake")]
        [HideInInspector, SerializeField] private bool useScreenShake;
        [HideInInspector, SerializeField, Range(0.0f, 100.0f)] private float ssDampingSpeed;
        [HideInInspector, SerializeField, Range(0.0f, 10.0f)] private float ssMagnitude;
        [HideInInspector, SerializeField, Range(0.0f, 10.0f)] private float ssDuration;
        [HideInInspector, SerializeField] private  UnityEvent<float, float, float> onScreenShake;

        private void Awake()
        {
            // copy base blkColor in list
            foreach (var r in blkRenderers)
            {
                var size = r.materials.Length;
                var colors = new Color[size];
                for (var i = 0; i < size; i++) colors[i] = r.materials[i].color;
                _blkMaterialsColors.Add(colors);
            }
        }

        #region ScreenShake

        public void Shake()
        {
            if (!useScreenShake) return;
            onScreenShake?.Invoke(ssDuration, ssMagnitude, ssDampingSpeed);
        }

        #endregion

        #region KnockBack
        
        public void KnockBack(Damager damager)
        {
            if (!useKnockBack) return;
            StartCoroutine(KnockBacking(damager));
        }

        private IEnumerator KnockBacking(Damager damager)
        {
            if (!TryGetComponent(out Rigidbody2D rb) || !TryGetComponent(out Collider2D col) || !damager) yield break;
            
            onKnockBackStart?.Invoke();
            Vector2 closestPoint = damager.enterFirstContactPosition;
            Vector2 colliderOffset = transform.TransformPoint(col.offset);
            Vector2 direction = (closestPoint - colliderOffset).normalized;
            rb.AddForce(rb.mass * kbForce * direction, ForceMode2D.Impulse);
            
            yield return new WaitForSeconds(kbDuration);
            onKnockBackEnd?.Invoke();
        }

        #endregion
        
        #region Blink
        
        public void Blink()
        {
            if (!useBlink) return;
            StartCoroutine(Blinking());
        }

        private IEnumerator Blinking()
        {
            float timer = blkDuration;
            float fixedPingPong = blkDuration / blkCount;
            float pingPongTimer = fixedPingPong;

            while (timer > 0.0f)
            {
                timer -= Time.deltaTime;
                pingPongTimer -= Time.deltaTime;
                float lerp = Mathf.Clamp01(pingPongTimer / fixedPingPong);
                float lerpIntensity = (lerp * blkIntensity) + 1.0f;

                for (var rIndex = 0; rIndex < blkRenderers.Length; rIndex++)
                {
                    var r = blkRenderers[rIndex];
                    var colors = _blkMaterialsColors[rIndex];

                    for (var mIndex = 0; mIndex < r.materials.Length; mIndex++)
                    {
                        var material = r.materials[mIndex];
                        material.color = timer <= 0.0f ? colors[mIndex] : blkColor * lerpIntensity;
                    }
                }

                if (pingPongTimer <= 0.0f && timer > 0.0f)
                {
                    pingPongTimer = fixedPingPong;
                }

                yield return null;
            }
        }

        #endregion
    }
}