using System.Collections;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ballistic : MonoBehaviour
    {
        public float gravity = 9.81f;
        public float height = 2f;

        private Rigidbody _rigidbody;
        private LaunchData _launchData;

        private Coroutine _updateVelocity;
        private WaitForFixedUpdate _waitForFixedUpdate;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _waitForFixedUpdate = new WaitForFixedUpdate();
        }

        public void AimForImpact(Transform target)
        {
            if (!_rigidbody || !target) return;

            _launchData = LaunchData.Calculate(gravity, height, target.position, _rigidbody.position);
        }

        public void Apply()
        {
            if (!_rigidbody) return;

            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = _launchData.initialVelocity;

            _updateVelocity = StartCoroutine(UpdateVelocity());
        }

        public void Stop()
        {
            if (_updateVelocity == null) return;

            StopCoroutine(_updateVelocity);
        }

        private IEnumerator UpdateVelocity()
        {
            _rigidbody.AddForce(Vector3.up * (gravity * -2f));
            if (_rigidbody.velocity.normalized.magnitude >= Mathf.Epsilon)
            {
                _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation,
                    Quaternion.LookRotation(_rigidbody.velocity.normalized,
                        Vector3.up), _rigidbody.velocity.magnitude);
            }

            yield return _waitForFixedUpdate;
        }
    }
}