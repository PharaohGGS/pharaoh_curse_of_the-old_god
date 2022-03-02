using System.Collections;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ballistic : MonoBehaviour
    {
        public float gravity = 9.81f;
        public float height = 2f;

        [field: SerializeField] public Damager damager { get; set; }

        private Rigidbody2D _rb2D;
        private LaunchData _launchData;

        private Coroutine _updateVelocity;
        private WaitForFixedUpdate _waitForFixedUpdate;
        
        private void Awake()
        {
            if (!damager && TryGetComponent(out Damager d))
            {
                damager = d;
            }

            _rb2D = GetComponent<Rigidbody2D>();
            _waitForFixedUpdate = new WaitForFixedUpdate();
        }

        public void AimForImpact(Damager d, Transform target)
        {
            if (damager != d || !target) return;

            if (!_rb2D)
            {
                LogHandler.SendMessage($"[{name}] doesn't have rigidbody to perform ballistic", MessageType.Warning);
                return;
            }

            _launchData = LaunchData.Calculate(gravity, height, target.position + Vector3.up, _rb2D.position);
        }

        public void Apply()
        {
            if (!_rb2D) return;

            _rb2D.bodyType = RigidbodyType2D.Dynamic;
            _rb2D.velocity = _launchData.initialVelocity;

            _updateVelocity = StartCoroutine(UpdateVelocity());
        }

        public void Stop()
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
    }
}