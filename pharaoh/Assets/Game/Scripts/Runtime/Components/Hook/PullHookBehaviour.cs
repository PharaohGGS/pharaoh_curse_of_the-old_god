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

        private MovingBlock movingBlock;
        private Rigidbody2D mbRigidbody;
        
        private PlayerMovement _movement;

        private bool _isMoving;
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
                Vector2 position = transform.position;
                Vector2 hookPosition = _hook.transform.position;
                float pullOffset = _hook.pullData.offset;
                return position.x > (hookPosition.x + pullOffset) || position.x < (hookPosition.x - pullOffset);
            }
        }

        /// <summary>
        /// test if something is between the block & the player
        /// </summary>
        private bool isBlocked 
        {
            get
            {
                if (!_movement || !_hook) return false;
                
                Vector2 position = transform.position;
                Vector2 hookPosition = _hook.transform.position;
                Vector2 direction = hookPosition - position;
                bool isFacingRight = _movement.isFacingRight;

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
            movingBlock = TryGetComponent(out MovingBlock mb) ? mb : GetComponentInParent<MovingBlock>();
            movingBlock.TryGetComponent(out mbRigidbody);
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
            if (!_isCurrentTarget || !_hook) return;

            if (!_hasBeenReleased)
            {
                if (!canBePulled || isBlocked || !movingBlock.isGrounded)
                {
                    _hasBeenReleased = true;
                }

                return;
            }
            
            // only when has been released
            if (_isMoving) return;

            Release();
        }
        
        private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!_isCurrentTarget) return;
            if (!_movement || (_movement && !_movement.IsHookedToBlock)) return;

            var axis = _input.CharacterControls.Move.ReadValue<Vector2>();
            if (Mathf.Abs(axis.x - Mathf.Epsilon) <= Mathf.Epsilon) return;
            _hasBeenReleased = true;
        }

        private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!_isCurrentTarget) return;
            if (!_movement || (_movement && !_movement.IsHookedToBlock)) return;
            _hasBeenReleased = true;
        }

        private void OnDash(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!_isCurrentTarget) return;
            if (!_movement || (_movement && !_movement.IsHookedToBlock)) return;
            _hasBeenReleased = true;
        }

        public override void Release()
        {
            base.Release();
            if (_pullCoroutine != null) StopCoroutine(_pullCoroutine);
            mbRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            if (_movement) _movement.IsPullingBlock = _movement.IsHookedToBlock = false;
            _hasBeenReleased = false;
            _isMoving = false;
        }

        public override void Interact(HookCapacity hook, GameObject target)
        {
            base.Interact(hook, target);
            if (!_isCurrentTarget) return;

            if (!_hook.TryGetComponent(out _movement))
            {
                LogHandler.SendMessage("No movement on player", MessageType.Warning);
                _isCurrentTarget = false;
                return;
            }

            if (!canBePulled || !movingBlock.isGrounded || _movement.IsPullingBlock)
            {
                _isCurrentTarget = false;
                return;
            }

            if (_hook.TryGetComponent(out Rigidbody2D rb)) rb.velocity = Vector2.zero;
            _movement.IsPullingBlock = _movement.IsHookedToBlock = true;
            _pullCoroutine = StartCoroutine(Pull());
        }

        private System.Collections.IEnumerator Pull()
        {
            if (!movingBlock || !mbRigidbody) yield break;
            
            hookIndicator?.SetActive(false);
            float currentTime = 0f;
            float maxMovement = _hook.pullData.maxMovement;
            float duration = _hook.pullData.duration;
            float force = _hook.pullData.force;
            AnimationCurve curve = _hook.pullData.curve;

            //mbRigidbody.gravityScale = 0f;
            Vector2 startPosition = mbRigidbody.position;

            Vector2 direction = _hook.transform.position - transform.position;
            Vector2 velocityX = (direction.x < 0.0f ? Vector2.left : Vector2.right) * maxMovement;
            Vector2 velocityY = Vector2.up * mbRigidbody.velocity.y;

            Vector2 endPosition = startPosition + velocityX + velocityY;

            //mbRigidbody.velocity = velocityX + velocityY;
            mbRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            _isMoving = true;
            while (currentTime < duration)
            {
                mbRigidbody.MovePosition(Vector2.Lerp(startPosition, endPosition, curve.Evaluate(currentTime / duration)));
                currentTime = Mathf.MoveTowards(currentTime, duration, Time.fixedDeltaTime * force);
                
                if (currentTime >= duration && _input.CharacterActions.Hook.IsPressed())
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
            _movement?.OnPullEnd();
        }
    }
}