using System;
using System.Collections;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ballistic : MonoBehaviour
    {
        [SerializeField] private bool rotate;
        [SerializeField] private bool usePhysics = true;
        public float gravity = 9.81f;
        public float height = 2f;

        [Header("Non Physics")]
        private Vector3 _velocity;

        [Header("Physics")]
        private Rigidbody2D _rb2D;
        private LaunchData _launchData;


        [Header("Routine")]
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

            //_rb2D.bodyType = RigidbodyType2D.Dynamic;
            //_rb2D.velocity = _velocity;

            _updateVelocity = StartCoroutine(UpdatePhysicsVelocity());
        }

        public void Disable()
        {
            if (_updateVelocity == null) return;

            StopCoroutine(_updateVelocity);
        }

        /// <summary>
        /// TO CHECK : https://devsplorer.wordpress.com/2021/01/09/angular-velocity-of-rigidbody-in-the-direction-of-movement-a-unity-tutorial/
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdatePhysicsVelocity()
        {
            while (true)
            {
                _rb2D.AddForce(Vector2.up * (gravity * -2f));
                if (rotate && _rb2D.velocity.normalized.magnitude >= Mathf.Epsilon)
                {
                    float velocityAngle = Mathf.Atan2(_rb2D.velocity.y, _rb2D.velocity.x) * Mathf.Rad2Deg;
                    _rb2D.rotation = Mathf.MoveTowardsAngle(_rb2D.rotation, velocityAngle, gravity * 2f);
                }

                yield return _waitForFixedUpdate;
            }
        }

        //public void AimForImpact(Transform target)
        //{
        //    if (!target) return;

        //    if (!_rb2D)
        //    {
        //        LogHandler.SendMessage($"[{name}] doesn't have rigidbody to perform ballistic", MessageType.Warning);
        //        return;
        //    }

        //    _velocity = !usePhysics 
        //        ? target.position - transform.position 
        //        : LaunchData.Calculate(Mathf.Max(gravity, Mathf.Epsilon), Mathf.Max(height, Mathf.Epsilon), 
        //            (Vector2)target.position + Vector2.up, _rb2D.position).initialVelocity;
        //}
    }
}