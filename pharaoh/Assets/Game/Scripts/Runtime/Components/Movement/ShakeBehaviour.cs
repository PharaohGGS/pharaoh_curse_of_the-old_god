using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Pharaoh.Components.Movement
{
    public class ShakeBehaviour : MonoBehaviour
    {
        [SerializeField] private bool useCinemachineCamera;
        [SerializeField] private NoiseSettings noiseSettings;

        private CinemachineVirtualCamera _virtualCamera;
        private Vector3 _initialPosition;
        
        private void OnEnable()
        {
            _initialPosition = transform.localPosition;
        }

        public void ChangeVirtualCamera(ICinemachineCamera incoming, ICinemachineCamera outgoing)
        {
            CinemachineVirtualCamera incomingCamera = incoming as CinemachineVirtualCamera;
            CinemachineVirtualCamera outgoingCamera = outgoing as CinemachineVirtualCamera;

            outgoingCamera?.DestroyCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            // add Perlin component & set the amplitude to 0 to avoid always shaking
            var cinemachineBasicMultiChannelPerlin = incomingCamera?.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_NoiseProfile = noiseSettings;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0.0f;
            _virtualCamera = incomingCamera;
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
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
                    _virtualCamera?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

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