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
        private RaycastHit2D[] _hits;
        
        /// <summary>
        /// test if you can pull the block 
        /// </summary>
        private bool canBePulled
        {
            get
            {
                if (!_hook) return false;
                float offset = _hook.pullData.offset;
                Vector2 position = transform.position;
                Vector2 hookPosition = _hook.transform.position;
                return position.x > (hookPosition.x + offset) || position.x < (hookPosition.x - offset);
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
                Vector2 hookPosition = _hook.transform.position;
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

            _hits = new RaycastHit2D[2];
            _movingBlock = TryGetComponent(out MovingBlock mb) ? mb : GetComponentInParent<MovingBlock>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _input.CharacterControls.Move.performed += OnMove;
            _input.CharacterControls.Jump.started += OnJump;
            _input.CharacterControls.Dash.started += OnDash;
        }

        protected override void OnDisable()
        {
            _input.CharacterControls.Move.performed -= OnMove;
            _input.CharacterControls.Jump.started -= OnJump;
            _input.CharacterControls.Dash.started -= OnDash;
            base.OnDisable();
        }

        private void FixedUpdate()
        {
            if (!isCurrentTarget || !_hook || !_movingBlock) return;

            _hasBeenReleased = !canBePulled || isBlocked || !_movingBlock.isGrounded;
            if (_hasBeenReleased) Release();
        }
        
        private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!isCurrentTarget || !_movingBlock || !_movingBlock.isHooked) return;

            var axis = _input.CharacterControls.Move.ReadValue<Vector2>();
            if (Mathf.Abs(axis.x - Mathf.Epsilon) <= Mathf.Epsilon) return;
            _hasBeenReleased = true;
        }

        private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!isCurrentTarget || !_movingBlock || !_movingBlock.isHooked) return;
            _hasBeenReleased = true;
        }

        private void OnDash(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
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

            if (!canBePulled || !_movingBlock || !_movingBlock.isGrounded || _movingBlock.isPulled)
            {
                Release();
                return;
            }

            hookIndicator?.SetActive(false);
            _pullCoroutine = StartCoroutine(Pull());
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

            Vector2 direction = _hook.transform.position - transform.position;
            Vector2 velocityX = (direction.x < 0.0f ? Vector2.left : Vector2.right) * maxMovement;

            Vector2 endPosition = startPosition + velocityX;
            
            while (currentTime < duration)
            {
                nextPosition = Vector2.Lerp(startPosition, endPosition, curve.Evaluate(currentTime / duration));
                currentTime = Mathf.MoveTowards(currentTime, duration, Time.fixedDeltaTime * force);
                Perform();
                
                if (!_hasBeenReleased && currentTime >= duration && _input.CharacterActions.Hook.IsPressed())
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