using System;
using Pharaoh.Gameplay;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Tools.Debug;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;
using UnityEngine.InputSystem;
using MessageType = Pharaoh.Tools.Debug.MessageType;
using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;

namespace Pharaoh.Gameplay
{
    public class GrappleHookCapacity : HookCapacity
    {
        [SerializeField, Tooltip("grapple speed toward target in m/s")]
        public float grappleSpeed;
        [SerializeField, Tooltip("grapple movement curve from a to b")]
        public AnimationCurve smoothCurve;
        
        public UnityEvent<GrappleHookCapacity, GrappleHookBehaviour> onGrappleStart = new UnityEvent<GrappleHookCapacity, GrappleHookBehaviour>();
        public UnityEvent<GrappleHookCapacity, GrappleHookBehaviour> onGrappleEnd = new UnityEvent<GrappleHookCapacity, GrappleHookBehaviour>();

        private bool isBlocked 
        {
            get
            {
                if (!_currentTarget) return false;
                Vector2 direction = _currentTarget.transform.position - transform.position;
                return Physics2D.RaycastAll(transform.position,
                    direction.normalized, direction.magnitude, whatIsObstacle).Length > 0;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _input.CharacterActions.Hook.performed += OnHook;
            _input.CharacterControls.Move.performed += OnMove;
            _input.CharacterControls.Jump.started += OnJump;
            _input.CharacterControls.Dash.started += OnDash;
        }

        protected override void OnDisable()
        {
            _input.CharacterActions.Hook.performed -= OnHook;
            _input.CharacterControls.Move.performed -= OnMove;
            _input.CharacterControls.Jump.started -= OnJump;
            _input.CharacterControls.Dash.started -= OnDash;
            base.OnDisable();
        }

        private void FixedUpdate()
        {
            // cancel if raycast hit another object than target
            if (isBlocked) OnRelease();
        }
        
        private void OnHook(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
        {
            // unhook the current hooked object if there is one
            if (_currentTarget) OnRelease();
            // hook the nearest hookable objects if there is one
            if (!_potentialTarget || _movement.IsStunned) return;
            
            _currentTarget = _potentialTarget;

            if (!_currentTarget.TryGetComponent(out GrappleHookBehaviour ghb)) return;

            Debug.Log($"Start grapple to {_currentTarget.name}");
            onGrappleStart?.Invoke(this, ghb);
            // select the target based on the direction the player's facing
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            var axis = _input.CharacterControls.Move.ReadValue<Vector2>();

            if (axis.y >= -0.8f || !_currentTarget) return;

            OnRelease();
        }

        private void OnJump(InputAction.CallbackContext obj)
        {
            if (!_currentTarget) return;
            OnRelease();
        }

        private void OnDash(InputAction.CallbackContext obj)
        {
            if (!_currentTarget) return;
            OnRelease();
        }

        #region Editor Debug

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_movement) return;

            // Draws the best target to the right(red if not the faced direction)
            Gizmos.color = _movement.isFacingRight
                ? new Color(1f, 0.7531517f, 0f, 1f)
                : new Color(1f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetRight != null)
                Gizmos.DrawLine(transform.position, _bestTargetRight.transform.position);

            // Draws the best target to the left (red if not the faced direction)
            Gizmos.color = !_movement.isFacingRight
                ? new Color(1f, 0.7531517f, 0f, 1f)
                : new Color(1f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetLeft != null)
                Gizmos.DrawLine(transform.position, _bestTargetLeft.transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            // Draws a disc around the player displaying the targeting range
            Handles.color = new Color(1f, 0.7531517f, 0f, 1f);
            Handles.DrawWireDisc(transform.position, Vector3.forward, overlapingRadius);
        }
#endif

        #endregion
    }
}