using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Pharaoh.Components.Movement
{
    public class ShakeBehaviour : MonoBehaviour
    {
        [SerializeField] private bool useCinemachineCamera;
        
        private CinemachineBrain _cinemachineBrain;
        private Vector3 _initialPosition;

        private void Awake()
        {
            if (!useCinemachineCamera || !TryGetComponent(out _cinemachineBrain))
                Debug.LogWarning("No CinemachineBrain component found.");
        }

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
            float totalDuration = duration;

            while (duration > 0.0f)
            {
                CinemachineVirtualCamera virtualCamera = _cinemachineBrain?.ActiveVirtualCamera as CinemachineVirtualCamera;
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                var currentMagnitude = Mathf.Lerp(magnitude, 0f, 1 - (duration / totalDuration));
                if (useCinemachineCamera && cinemachineBasicMultiChannelPerlin)
                {
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = currentMagnitude;
                }
                else
                {
                    transform.localPosition = _initialPosition + Random.insideUnitSphere * currentMagnitude;
                }

                duration = Mathf.Max(0.0f, duration - Time.deltaTime * dampingSpeed);

                if (duration <= 0.0f)
                {
                    duration = 0f;

                    if (useCinemachineCamera && cinemachineBasicMultiChannelPerlin)
                    {
                        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0.0f;
                    }
                    else
                    {
                        transform.localPosition = _initialPosition;
                    }
                }

                yield return null;
            }
        }
    }
}