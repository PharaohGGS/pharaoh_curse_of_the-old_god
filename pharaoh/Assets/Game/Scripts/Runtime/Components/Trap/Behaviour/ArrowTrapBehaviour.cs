using System;
using System.Collections;
using Pharaoh.Gameplay.Components;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(DamagerPool))]
    public class ArrowTrapBehaviour : TrapBehaviour<DistanceTrapData>
    {
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        
        private DamagerPool _pool;
        private ArrowSound _arrowSound;

        private void Awake()
        {
            _pool = GetComponent<DamagerPool>();
            _arrowSound = GetComponentInParent<ArrowSound>();
            Reset();
        }

        public override void Enable()
        {
            // don't start trap when there isn't any target or already processing
            if (_isStarted) return;
            
            _isStarted = true;
            StartCoroutine(Action());
        }

        public override void Disable()
        {
            if (!_isStarted) return;
            Reset();
        }

        public override void Reset()
        {
            StopAllCoroutines();
            _isStarted = false;
        }

        private IEnumerator Action()
        {
            var delay = new WaitForSeconds(data.delay);
            var timeOut = new WaitForSeconds(data.timeOut);

            // wait a delay before activate the trap
            if (!data.oneTimeDelay) yield return delay;

            // trap launch arrows
            var damager = _pool.Get();

            if (damager.TryGetComponent(out DamagerReturnToPool returnToPool))
            {
                Debug.Log("----Playing arrow sound");
                _arrowSound.ArrowActivationSound();
                returnToPool.StartLifeTimeCountDown(data.lifeTime);
            }

            if (damager.TryGetComponent(out Rigidbody2D rb2D))
            {
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                rb2D.AddTorque(transform.rotation.z * Mathf.Deg2Rad * rb2D.inertia);
                rb2D.AddForceAtPosition(transform.up * data.initialVelocity, transform.position,
                    ForceMode2D.Impulse);
            }

            LogHandler.SendMessage($"{name} shooting {damager.name}", MessageType.Warning);

            // timeOut after hiding
            yield return timeOut;
                
            if (!_isStarted) yield break;
            StartCoroutine(Action());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * data.initialVelocity);
        }
    }
}