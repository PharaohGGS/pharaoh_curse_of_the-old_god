using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(VisualEffect))]
    public class HookIndicatorVFX : MonoBehaviour
    {
        private VisualEffect _vfx;

        [System.Serializable]
        public struct FadeLifeTime {
            public float minLifeTime;
            public float maxLifeTime;
            public float speed;
        }
        
        public FadeLifeTime fadeIn;
        public FadeLifeTime fadeOut;

        public Coroutine _coroutine;

        private void Awake()
        {
            _vfx = GetComponent<VisualEffect>();
        }

        public void FadeIn()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Fading(fadeIn));
        }

        public void FadeOut()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Fading(fadeOut));
        }

        private IEnumerator Fading(FadeLifeTime fade)
        {
            bool minEnded = false;
            bool maxEnded = false;

            while (true)
            {
                if (minEnded && maxEnded) break;

                float currentMin = _vfx.GetFloat("MinLifetime");
                minEnded = Mathf.Abs(currentMin - fade.minLifeTime) <= Mathf.Epsilon;
                if (!minEnded) _vfx.SetFloat("MinLifetime", Mathf.MoveTowards(currentMin, fade.minLifeTime, Time.deltaTime * fade.speed));
                                
                float currentMax = _vfx.GetFloat("MaxLifetime");
                maxEnded = Mathf.Abs(currentMax - fade.maxLifeTime) <= Mathf.Epsilon;
                if (!maxEnded) _vfx.SetFloat("MaxLifetime", Mathf.MoveTowards(currentMax, fade.maxLifeTime, Time.deltaTime * fade.speed));
                
                yield return null;
            }

            _vfx.SetFloat("MinLifetime", fade.minLifeTime);
            _vfx.SetFloat("MaxLifetime", fade.maxLifeTime);
        }
    }
}