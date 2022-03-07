using System;
using System.Collections;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ballistic : MonoBehaviour
    {
        [SerializeField] private bool usePhysics = true;

        public float gravity = 9.81f;
        public float height = 2f;
        
        private Rigidbody2D _rb2D;
        private LaunchData _launchData;

        private Coroutine _updateVelocity;
        private WaitForFixedUpdate _waitForFixedUpdate;

        private void Awake()
        {
            _rb2D = GetComponent<Rigidbody2D>();
            _waitForFixedUpdate = new WaitForFixedUpdate();
        }

        public void Enable()
        {
            if (!_rb2D) return;

            _rb2D.bodyType = RigidbodyType2D.Dynamic;
            _rb2D.velocity = _launchData.initialVelocity;

            _updateVelocity = StartCoroutine(UpdateVelocity());
        }

        public void Disable()
        {
            if (_updateVelocity == null) return;

            StopCoroutine(_updateVelocity);
        }

        private IEnumerator UpdateVelocity()
        {
            while (true)
            {
                _rb2D.AddForce(Vector2.up * (gravity * -2f));
                if (_rb2D.velocity.normalized.magnitude >= Mathf.Epsilon)
                {
                    float velocityAngle = Mathf.Atan2(_rb2D.velocity.y, _rb2D.velocity.x) * Mathf.Rad2Deg;
                    _rb2D.rotation = Mathf.MoveTowardsAngle(_rb2D.rotation, velocityAngle,_rb2D.velocity.magnitude);
                }

                yield return _waitForFixedUpdate;
            }
        }

        public void AimForImpact(Transform target)
        {
            if (!target) return;

            if (!_rb2D)
            {
                LogHandler.SendMessage($"[{name}] doesn't have rigidbody to perform ballistic", MessageType.Warning);
                return;
            }

            _launchData = LaunchData.Calculate(gravity, height, (Vector2)target.position + Vector2.up, _rb2D.position);
        }
    }
}