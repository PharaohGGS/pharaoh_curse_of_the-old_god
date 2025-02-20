﻿using System.Collections;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public class SpikeTrapBehaviour : TrapBehaviour<MeleeTrapData>
    {
        [SerializeField] private GameObject mesh;
        [SerializeField] private Transform showingTransform;
        [SerializeField] private Transform hidingTransform;
        private bool _firstActivation;

        private Collider2D _col;
        private Rigidbody2D _rb;

        private TrapSound _spikeSound;

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        protected void Awake()
        {
            // at start hide gear
            if (TryGetComponent(out _col)) _col.enabled = false;
            if (TryGetComponent(out _rb) && hidingTransform) Reset();

            _spikeSound = GetComponent<TrapSound>();
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
            if (data.isTimed)
            {
                Reset();
            }
            else
            {
                _isStarted = false;
            }
        }

        private void EnableMeshAndCollision(bool value)
        {
            if (_col) _col.enabled = value;
            mesh?.SetActive(value);
        }

        public override void Reset()
        {
            StopAllCoroutines(); // kind of break the synchro of oneTimeDelayed 
            EnableMeshAndCollision(false);
            if (data.oneTimeDelay) _firstActivation = true;
            StartCoroutine(Move(data.hidingSpeed * 100f, _rb.position, hidingTransform.position));
            _isStarted = false;
        }

        private IEnumerator Action()
        {
            var delay = new WaitForSeconds(data.delay);
            var lifeTime = new WaitForSeconds(data.lifeTime);
            var timeOut = new WaitForSeconds(data.timeOut);

            var show = Move(data.showingSpeed, hidingTransform.position, showingTransform.position);
            var hide = Move(data.hidingSpeed, showingTransform.position, hidingTransform.position);
            
            // wait a delay before activate the trap
            if (!data.oneTimeDelay || _firstActivation)
            {
                _firstActivation = false;
                yield return delay;
            }

            // when showing, activate collider
            EnableMeshAndCollision(true);
            Debug.Log("----Play spikes");
            _spikeSound.ActivationSound();
            yield return StartCoroutine(show);

            // after showing wait some lifeTime
            yield return lifeTime;

            // when hiding, disable collider after finish hiding
            yield return StartCoroutine(hide);
            EnableMeshAndCollision(false);

            // timeOut after hiding
            yield return timeOut;
            
            if (!_isStarted) yield break;
            StartCoroutine(Action());
        }

        private IEnumerator Move(float speed, Vector2 start, Vector2 end)
        {
            if (!_rb) yield break;

            float currentTime = 0f;
            var curve = data.curve;
            
            float maxDistance = Vector2.Distance(start, end);
            float duration = maxDistance / speed;
            Vector2 nextPosition = start;

            // movement
            while (currentTime < duration)
            {
                nextPosition = Vector2.Lerp(start, end, curve.Evaluate(currentTime / duration));
                currentTime = Mathf.MoveTowards(currentTime, duration, Time.fixedDeltaTime * speed);
                _rb.MovePosition(nextPosition);
                yield return _waitForFixedUpdate;
            }

            nextPosition = Vector2.Lerp(start, end, curve.Evaluate(1)); 
            _rb.MovePosition(nextPosition);
        }
    }
}