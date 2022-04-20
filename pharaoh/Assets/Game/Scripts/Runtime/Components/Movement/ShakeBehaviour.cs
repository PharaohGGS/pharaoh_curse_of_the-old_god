using System.Collections;
using UnityEngine;

namespace Pharaoh.Components.Movement
{
    public class ShakeBehaviour : MonoBehaviour
    {
        private Vector3 _initialPosition;

        private void OnEnable()
        {
            _initialPosition = transform.localPosition;
        }

        public void Shake(float duration, float magnitude, float dampingSpeed)
        {
            StartCoroutine(Shaking(duration, magnitude, dampingSpeed));
        }

        private IEnumerator Shaking(float duration, float magnitude, float dampingSpeed)
        {
            while (duration > 0.0f)
            {
                transform.localPosition = _initialPosition + Random.insideUnitSphere * magnitude;
                duration = Mathf.Max(0.0f, duration - Time.deltaTime * dampingSpeed);
                yield return null;
            }

            duration = 0f;
            transform.localPosition = _initialPosition;
        }
    }
}