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
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerMovement))]
    public class HookTargeting : Targeting
    {
        private PlayerInput _playerInput;
        private PlayerMovement _playerMovement;
        
        [Header("Events")] 
        
        public UnityEvent<HookTargeting, GameObject> onHookFixed = new UnityEvent<HookTargeting, GameObject>();
        public UnityEvent<HookTargeting, GameObject> onHookReleased = new UnityEvent<HookTargeting, GameObject>();
        public UnityEvent<HookTargeting, GameObject> onHookBestTargetSelected = new UnityEvent<HookTargeting, GameObject>();
        
        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _playerInput = new PlayerInput();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.CharacterActions.Hook.performed += OnHook;
            _playerInput.CharacterControls.Move.performed += OnMove;
            _playerInput.CharacterControls.Jump.started += OnJump;
            _playerInput.CharacterControls.Dash.started += OnDash;
        }

        private void OnDisable()
        {
            _playerInput.CharacterActions.Hook.performed -= OnHook;
            _playerInput.CharacterControls.Move.performed -= OnMove;
            _playerInput.CharacterControls.Jump.started -= OnJump;
            _playerInput.CharacterControls.Dash.started -= OnDash;
            _playerInput.Disable();
        }

        private void Update()
        {
            SearchTargets();

            GameObject bestTargetSelected = _playerMovement.IsFacingRight switch
            {
                //facing right with right target
                true when _bestTargetRight != null => _bestTargetRight,
                //facing left with left target
                false when _bestTargetLeft != null => _bestTargetLeft,
                //facing right without right target
                true when _bestTargetRight == null => _bestTargetLeft != null ? _bestTargetLeft : null,
                //facing left without left target
                false when _bestTargetLeft == null => _bestTargetRight != null ? _bestTargetRight : null,
                // else
                _ => null
            };

            onHookBestTargetSelected?.Invoke(this, bestTargetSelected);
        }

        #region Editor Debug

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_playerMovement) return;

            // Draws the best target to the right(red if not the faced direction)
            Gizmos.color = _playerMovement.isFacingRight
                ? new Color(1f, 0.7531517f, 0f, 1f)
                : new Color(1f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetRight != null)
                Gizmos.DrawLine(transform.position, _bestTargetRight.transform.position);

            // Draws the best target to the left (red if not the faced direction)
            Gizmos.color = !_playerMovement.isFacingRight
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

#endregion

        #region Input Events

        private void OnMove(InputAction.CallbackContext obj)
        {
            var axis = _playerInput.CharacterControls.Move.ReadValue<Vector2>();

            if (axis.y >= -0.8f || !_currentTarget) return;

            Released();
        }

        private void OnJump(InputAction.CallbackContext obj)
        {
            if (!_currentTarget) return;
            Released();
        }

        private void OnDash(InputAction.CallbackContext obj)
        {
            if (!_currentTarget) return;
            Released();
        }

        private void OnHook(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
        {
            // unhook the current hooked object if there is one
            if (_currentTarget) Released();
            // hook the nearest hookable objects if there is one
            if ((_bestTargetLeft || _bestTargetRight) && !_playerMovement.IsStunned) Fixed();
        }

        #endregion

        public void Respawn()
        {
            Released();
        }

        private void Fixed()
        {
            _currentTarget = null;

            if (!_playerMovement.isFacingRight)
            {
                _currentTarget = _bestTargetRight && !_bestTargetLeft
                    ? _bestTargetRight : _bestTargetLeft;
            }
            else
            {
                _currentTarget = _bestTargetLeft && !_bestTargetRight
                    ? _bestTargetLeft : _bestTargetRight;
            }

            if (!_currentTarget) return;
            
            LogHandler.SendMessage($"hooking to {_currentTarget.name}", MessageType.Log);
            
            onHookFixed?.Invoke(this, _currentTarget);
        }

        private void Released()
        {
            LogHandler.SendMessage($"unhooking from {_currentTarget.name}", MessageType.Log);
            onHookReleased?.Invoke(this, _currentTarget);
        }

        public void ResetTarget() => _currentTarget = null;
    }
}