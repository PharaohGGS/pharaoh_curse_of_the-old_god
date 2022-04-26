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
        public float gravity = 9.81f;
        public float height = 2f;
        
        [Header("Physics")]
        private Rigidbody2D _rb2D;
        private Vector2 _velocityStart;
        
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
            _velocityStart = _rb2D.velocity;
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
                _rb2D.AddForce(_velocityStart.normalized * (gravity * -2f));
                if (rotate && _rb2D.velocity.magnitude >= Mathf.Epsilon)
                {
                    var velocity = _rb2D.velocity.normalized;
                    float velocityAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                    var diffAngle = (velocity.x > 0.0f ? -1 : 1) * Mathf.Abs(velocityAngle - _rb2D.rotation);
                    _rb2D.MoveRotation(_rb2D.rotation + diffAngle * Time.fixedDeltaTime);
                }

                yield return _waitForFixedUpdate;
            }
        }
    }
}