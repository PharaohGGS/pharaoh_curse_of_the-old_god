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

        private void Awake()
        {
            _pool = GetComponent<DamagerPool>();
        }

        public override void Activate(GameObject target)
        {
            bool isSameTarget = _currentTarget == target;
            if (!isSameTarget) _currentTarget = target;
            bool addDelay = !isSameTarget || !data.oneTimeDelay;
            StartCoroutine(Action(addDelay));
        }

        public override void Respawn()
        {
            // do nothing for now
        }

        private IEnumerator Action(bool addDelay)
        {
            var delay = new WaitForSeconds(data.delay);
            var timeOut = new WaitForSeconds(data.timeOut);

            isStarted = true;

            // wait a delay before activate the trap
            if (addDelay) yield return delay;
            
            // trap launch arrows
            var damager = _pool.Get();

            if (damager.TryGetComponent(out DamagerReturnToPool returnToPool))
            {
                returnToPool.StartLifeTimeCountDown(data.lifeTime);
            }

            if (damager.TryGetComponent(out Rigidbody2D rb2D))
            {
                //rb2D.rotation = transform.rotation.z;
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                var direction = (Vector2)_currentTarget.transform.position - rb2D.position;
                rb2D.AddTorque(transform.rotation.z * Mathf.Deg2Rad * rb2D.inertia);
                rb2D.AddForceAtPosition(transform.up * data.initialVelocity, transform.position, ForceMode2D.Impulse);
                //rb2D.velocity = Vector2.up * data.initialVelocity;
            }

            LogHandler.SendMessage($"{name} shooting {damager.name}", MessageType.Warning);
            
            // timeOut after hiding
            yield return timeOut;
            isStarted = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * data.initialVelocity);
        }
    }
}