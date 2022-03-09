using System;
using Pharaoh.Gameplay;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using MessageType = Pharaoh.Tools.Debug.MessageType;

namespace Pharaoh.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerMovement))]
    public class HookTargeting : Targeting
    {
        [Header("Movement")] 
        [SerializeField, Tooltip("Distance to hooked transform")] private float offsetHook = 0.01f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private AnimationCurve smoothCurve;
        private bool _isOnHook = false;
        
        private Coroutine _moveToHook;
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        private PlayerInput _playerInput;
        private Rigidbody2D _rigidbody;
        private PlayerMovement _playerMovement;

        private Vector3 _playerPositionOnHook;
        
        [Header("Events")] public UnityEvent onHook = new UnityEvent();
        public UnityEvent onUnHook = new UnityEvent();
        public UnityEvent onEndHookMovement = new UnityEvent();

        public GameObject hookIndicator;

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _playerInput = new PlayerInput();
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerMovement = GetComponent<PlayerMovement>();

            hookIndicator = Instantiate(hookIndicator);
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
            if (!_isOnHook && _currentTarget) return;
            SearchTargets();

            if (_playerMovement.IsFacingRight && _bestTargetRight != null) //facing right with right target
                hookIndicator.transform.position = _bestTargetRight.transform.position;
            else if (_playerMovement.IsFacingRight && _bestTargetRight == null) //facing right without right target
                hookIndicator.transform.position = _bestTargetLeft != null ? _bestTargetLeft.transform.position : new Vector3(-1000f, -1000f, 0f);
            else if (!_playerMovement.IsFacingRight && _bestTargetLeft != null) //facing left with left target
                hookIndicator.transform.position = _bestTargetLeft.transform.position;
            else if (!_playerMovement.IsFacingRight && _bestTargetLeft == null) //facing left without left target
                hookIndicator.transform.position = _bestTargetRight != null ? _bestTargetRight.transform.position : new Vector3(-1000f, -1000f, 0f);
            else //none of the above
                hookIndicator.transform.position = new Vector3(-1000f, -1000f, 0f);
        }

        #region Editor Debug

        private void OnDrawGizmos()
        {
            if (_rigidbody == null)
                return;

            // Draws the best target to the right (red if not the faced direction)
            Gizmos.color = _playerMovement.isFacingRight
                ? new Color(1f, 0.7531517f, 0f, 1f)
                : new Color(1f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetRight != null)
                Gizmos.DrawLine(_rigidbody.position, _bestTargetRight.transform.position);

            // Draws the best target to the left (red if not the faced direction)
            Gizmos.color = !_playerMovement.isFacingRight
                ? new Color(1f, 0.7531517f, 0f, 1f)
                : new Color(1f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetLeft != null)
                Gizmos.DrawLine(_rigidbody.position, _bestTargetLeft.transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            if (_rigidbody == null)
                return;

            // Draws a disc around the player displaying the targeting range
            Handles.color = new Color(1f, 0.7531517f, 0f, 1f);
            Handles.DrawWireDisc(_rigidbody.position, Vector3.forward, overlapingRadius);
        }

        #endregion

        #endregion

        #region Input Events
        
        private void OnMove(InputAction.CallbackContext obj)
        {
            var axis = _playerInput.CharacterControls.Move.ReadValue<Vector2>();

            if (axis.y >= -0.8f || !_currentTarget) return;

            UnHook();
        }

        private void OnJump(InputAction.CallbackContext obj)
        {
            if (!_currentTarget) return;
            UnHook();
        }

        private void OnDash(InputAction.CallbackContext obj)
        {
            if (!_currentTarget) return;
            UnHook();
        }

        private void OnHook(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
        {
            // unhook the current hooked object if there is one
            if (_currentTarget) UnHook();
            // hook the nearest hookable objects if there is one
            if ((_bestTargetLeft || _bestTargetRight) && !_playerMovement.IsStunned) Hook();
        }

        #endregion

        public void Respawn()
        {
            _isOnHook = false;
            UnHook();
        }

        private void Hook()
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

            if (!_currentTarget || !_rigidbody) return;

            _playerPositionOnHook = _currentTarget.transform.Find("PlayerPosition").transform.position;

            _rigidbody.velocity = Vector2.zero;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;

            LogHandler.SendMessage($"hooking to {_currentTarget.name}", MessageType.Log);

            _moveToHook = StartCoroutine(MoveToHook());
            onHook?.Invoke();
        }

        private void UnHook()
        {
            LogHandler.SendMessage($"unhooking from {_currentTarget.name}", MessageType.Log);
            if (_moveToHook != null) StopCoroutine(_moveToHook);

            _currentTarget = null;
            _isOnHook = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            onUnHook?.Invoke();
        }

        private System.Collections.IEnumerator MoveToHook()
        {
            if (!_currentTarget || !_rigidbody) yield break;

            hookIndicator.SetActive(false);

            Vector2 startPosition = _rigidbody.position;
            float current = 0f;

            while (Vector2.Distance(_playerPositionOnHook, _rigidbody.position) > offsetHook)
            {
                _isOnHook = false;
                Vector2 direction = (Vector2)_playerPositionOnHook - _rigidbody.position;
                float distance = Vector2.Distance(_playerPositionOnHook, _rigidbody.position);
                var hit2Ds = Physics2D.RaycastAll(_rigidbody.position, direction, distance, whatIsObstacle);

                if (hit2Ds.Length > 0) UnHook();

                current = Mathf.MoveTowards(current, 1f, moveSpeed * Time.fixedDeltaTime);
                _rigidbody.MovePosition(Vector2.Lerp(startPosition, _playerPositionOnHook, smoothCurve.Evaluate(current)));
                yield return _waitForFixedUpdate;
            }

            hookIndicator.SetActive(true);

            _isOnHook = true;
            onEndHookMovement?.Invoke();
        }

    }
}