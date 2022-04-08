using System;
using System.Linq;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Tools.Debug;
using Pharaoh.Tools.Inputs;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    public class PullHookBehaviour : HookBehaviour
    {
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private Coroutine _pullCoroutine;

        private MovingBlock _movingBlock;
        
        private bool _hasBeenReleased;
        
        /// <summary>
        /// test if you can pull the block 
        /// </summary>
        private bool canBePulled
        {
            get
            {
                if (!_hook) return false;
                float offset = _hook.pullData.offset;
                float x = transform.position.x;
                float hookX = _hook.transform.position.x;
                return x > (hookX + offset) || x < (hookX - offset);
            }
        }

        /// <summary>
        /// test if something is between the block & the player
        /// </summary>
        private bool isBlocked 
        {
            get
            {
                if (!_hook) return false;
                
                Vector2 position = transform.position;
                Vector2 hookPosition = _hook.center;
                Vector2 direction = hookPosition - position;
                bool isFacingRight = Mathf.Sign(-direction.x) >= 1f;

                if (direction.x >= 0.0f && !isFacingRight || direction.x <= 0.0f && isFacingRight)
                {
                    direction.x *= -1f;
                }
            
                int size = Physics2D.RaycastNonAlloc(hookPosition, direction.normalized, 
                    _hits, direction.magnitude, _hook.whatIsObstacle);
                return size > 0 && _hits[0].collider.gameObject == _hook.gameObject;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _movingBlock = TryGetComponent(out MovingBlock mb) 
                ? mb : GetComponentInParent<MovingBlock>();
        }

        protected void OnEnable()
        {
            inputs.hookInteractStartedEvent += OnHookInteractStart;
            inputs.movePerformedEvent += OnMove;
            inputs.jumpStartedEvent += OnJump;
            inputs.dashStartedEvent += OnDash;
        }

        protected void OnDisable()
        {
            inputs.hookInteractStartedEvent -= OnHookInteractStart;
            inputs.movePerformedEvent -= OnMove;
            inputs.jumpStartedEvent -= OnJump;
            inputs.dashStartedEvent -= OnDash;
        }

        private void FixedUpdate()
        {
            if (!isCurrentTarget || !_hook || !_movingBlock) return;

            if (!canBePulled || isBlocked || !_movingBlock.isGrounded)
            {
                _hasBeenReleased = true;
            }

            if (_hasBeenReleased) Release();
        }

        private void OnHookInteractStart()
        {
            if (isCurrentTarget) Release();
        }

        private void OnMove(Vector2 axis)
        {
            if (!isCurrentTarget || !_movingBlock || !_movingBlock.isHooked) return;
            if (Mathf.Abs(axis.x - Mathf.Epsilon) <= Mathf.Epsilon) return;
            _hasBeenReleased = true;
        }

        private void OnJump()
        {
            if (!isCurrentTarget || !_movingBlock || !_movingBlock.isHooked) return;
            _hasBeenReleased = true;
        }

        private void OnDash()
        {
            if (!isCurrentTarget || !_movingBlock || !_movingBlock.isHooked) return;
            _hasBeenReleased = true;
        }

        public override void Release()
        {
            base.Release();
            if (_pullCoroutine != null) StopCoroutine(_pullCoroutine);
            _hasBeenReleased = false;
        }

        public override void Interact(HookCapacity hook, GameObject target)
        {
            base.Interact(hook, target);
            if (!isCurrentTarget) return;

            // can't be pull when player is in air
            if (!canBePulled || !_movingBlock || !_movingBlock.isGrounded || _movingBlock.isPulled ||
                !_hook.TryGetComponent(out PlayerMovement movement) || !movement.IsGrounded)
            {
                Release();
                return;
            }
            
            Debug.Log($"{hook.name} hooking to {name}");
            _pullCoroutine = StartCoroutine(Pull());
            if (hookIndicator) hookIndicator.SetActive(false);
        }

        private System.Collections.IEnumerator Pull()
        {
            if (!_movingBlock) yield break;
            
            float currentTime = 0f;
            float maxMovement = _hook.pullData.maxMovement;
            float duration = _hook.pullData.duration;
            float force = _hook.pullData.force;
            AnimationCurve curve = _hook.pullData.curve;
            
            Vector2 startPosition = _movingBlock.transform.position;

            Vector2 direction = _hook.center - (Vector2)transform.position;
            Vector2 velocityX = (direction.x < 0.0f ? Vector2.left : Vector2.right) * maxMovement;

            Vector2 endPosition = startPosition + velocityX;
            
            while (currentTime < duration)
            {
                nextPosition = Vector2.Lerp(startPosition, endPosition, curve.Evaluate(currentTime / duration));
                currentTime = Mathf.MoveTowards(currentTime, duration, Time.fixedDeltaTime * force);
                Perform();
                
                if (!_hasBeenReleased && currentTime >= duration && inputs.hookInteract.IsPressed())
                {
                    currentTime = 0f;
                    startPosition = _movingBlock.transform.position;
                    endPosition = startPosition + velocityX;
                }

                yield return _waitForFixedUpdate;
            }
            
            // Cancels pulling if not holding the button
            End();
        }
    }
}