using System;
using System.Linq;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Tools.Inputs;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    public class PullHookBehaviour : HookBehaviour<PullHookCapacity>
    {
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private Coroutine _pullCoroutine;

        private MovingBlock movingBlock;
        private Rigidbody2D mbRigidbody;

        private readonly UnityEvent<PullHookCapacity, PullHookBehaviour> _onPullEnd = new UnityEvent<PullHookCapacity, PullHookBehaviour>();

        private bool _isMoving;

        protected override void Awake()
        {
            base.Awake();

            movingBlock = TryGetComponent(out MovingBlock mb) ? mb : GetComponentInParent<MovingBlock>();
            movingBlock.TryGetComponent(out mbRigidbody);
        }

        public override void Release(HookCapacity capacity)
        {
            if (_pullCoroutine == null) return;
            StartCoroutine(Release());
        }

        public override void Begin(PullHookCapacity capacity, HookBehaviour<PullHookCapacity> behaviour)
        {
            if (behaviour != this) return; 
            if (!movingBlock.isGrounded) return;
            _onPullEnd?.AddListener(capacity.OnPullEnd);
            _pullCoroutine = StartCoroutine(Pull(capacity));
        }

        private System.Collections.IEnumerator Release()
        {
            while (_isMoving) yield return null;

            StopCoroutine(_pullCoroutine);

            _onPullEnd?.RemoveAllListeners();
            
            mbRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            //mbRigidbody.velocity = Vector2.up * mbRigidbody.velocity.y;
            yield break;
        }

        private System.Collections.IEnumerator Pull(PullHookCapacity capacity)
        {
            if (!movingBlock || !mbRigidbody) yield break;
            
            hookIndicator?.SetActive(false);
            float currentTime = 0f;
            float maxMovement = capacity.pullMaxMovement;
            float duration = capacity.pullDuration;
            float force = capacity.pullForce;
            AnimationCurve curve = capacity.smoothCurve;

            //mbRigidbody.gravityScale = 0f;
            Vector2 startPosition = mbRigidbody.position;

            Vector2 direction = capacity.transform.position - transform.position;
            Vector2 velocityX = (direction.x < 0.0f ? Vector2.left : Vector2.right) * maxMovement;
            Vector2 velocityY = Vector2.up * mbRigidbody.velocity.y;

            Vector2 endPosition = startPosition + velocityX + velocityY;

            //mbRigidbody.velocity = velocityX + velocityY;
            mbRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            _isMoving = true;
            while (currentTime <= duration)
            {
                mbRigidbody.MovePosition(Vector2.Lerp(startPosition, endPosition, curve.Evaluate(currentTime / duration)));
                currentTime = Mathf.MoveTowards(currentTime, duration, Time.fixedDeltaTime * force);

                if (!movingBlock.isGrounded) break;
                if (currentTime >= duration && !capacity.canPull) break;
                if (currentTime >= duration && capacity.isPullPressed)
                {
                    currentTime = 0f;
                    startPosition = mbRigidbody.position;
                    endPosition = startPosition + velocityX + velocityY;
                }

                yield return _waitForFixedUpdate;
            }

            _isMoving = false;
            // Cancels pulling if not holding the button
            mbRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            mbRigidbody.velocity = velocityY;
            //mbRigidbody.gravityScale = 1f;
            _onPullEnd?.Invoke(capacity, this);
        }
    }
}