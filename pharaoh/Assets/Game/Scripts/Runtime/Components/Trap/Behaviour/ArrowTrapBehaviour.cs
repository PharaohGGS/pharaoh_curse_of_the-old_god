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
            StartCoroutine(Action(!isSameTarget));
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
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                var direction = (Vector2)_currentTarget.transform.position - rb2D.position;
                rb2D.AddForce(direction.normalized * data.initialVelocity, ForceMode2D.Impulse);
            }

            LogHandler.SendMessage($"{name} shooting {damager.name}", MessageType.Warning);
            
            // timeOut after hiding
            yield return timeOut;
            isStarted = false;
        }
    }
}