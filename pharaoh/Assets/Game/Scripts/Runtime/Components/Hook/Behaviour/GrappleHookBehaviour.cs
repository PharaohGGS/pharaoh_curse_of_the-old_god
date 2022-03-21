using System;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Tools.Debug;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public class GrappleHookBehaviour : HookBehaviour
    {
        [SerializeField, Tooltip("target position the player is going to be at the end")]
        private Transform finalMoveTarget;
        
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private Coroutine _moveToCoroutine;

        private Rigidbody2D _hookRigidbody;
        private PlayerMovement _movement;
        private RaycastHit2D[] _hits;

        protected override void Awake()
        {
            base.Awake();
            _hits = new RaycastHit2D[2];
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

            // Cancel grapple when encounter an obstacle 
            Vector2 hookPosition = _hook.transform.position;
            Vector2 direction = (Vector2)transform.position - hookPosition;
            int size = Physics2D.RaycastNonAlloc(hookPosition, direction.normalized, 
                _hits, direction.magnitude, _hook.whatIsObstacle);
            if (size > 0)
            {
                Release();
            }
        }

        private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!_isCurrentTarget) return;

            var axis = _input.CharacterControls.Move.ReadValue<Vector2>();
            if (axis.y >= -0.8f) return;

            Release();
        }

        private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!_isCurrentTarget) return;
            Release();
        }

        private void OnDash(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            if (!_isCurrentTarget) return;
            Release();
        }

        public override void Release()
        {
            base.Release();
            if (_moveToCoroutine != null) StopCoroutine(_moveToCoroutine);
            if (!_hookRigidbody) return;
            _hookRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        public override void Interact(HookCapacity hook, GameObject target)
        {
            base.Interact(hook, target);
            if (!_isCurrentTarget) return;

            if (!_hook.TryGetComponent(out _movement))
            {
                LogHandler.SendMessage("No movement on player", MessageType.Warning);
            }

            if (!_hook.TryGetComponent(out _hookRigidbody))
            {
                LogHandler.SendMessage("No rigidbody on player", MessageType.Warning);
            }
            
            hookIndicator?.SetActive(false);
            _moveToCoroutine = StartCoroutine(Grapple());
        }

        private System.Collections.IEnumerator Grapple()
        {
            if (!_hook) yield break;

            float speed = _hook.grappleData.speed;
            AnimationCurve curve = _hook.grappleData.curve;
            Vector2 startPosition = _hook.transform.position;

            float maxDistance = Vector2.Distance(startPosition, finalMoveTarget.position);
            float timeToTravel = maxDistance / speed;
            float currentTime = 0f;
            
            while (currentTime < timeToTravel)
            {
                _hook.transform.position =  Vector2.Lerp(startPosition, finalMoveTarget.position, curve.Evaluate(currentTime / timeToTravel));
                currentTime = Mathf.MoveTowards(currentTime, timeToTravel, Time.fixedDeltaTime * speed);
                yield return _waitForFixedUpdate;
            }
            
            _movement?.OnGrappleEnd();
            if (!_hookRigidbody) yield break;
            _hookRigidbody.velocity = Vector2.zero;
            _hookRigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}