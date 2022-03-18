using System;
using Pharaoh.Gameplay.Components.Movement;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;

namespace Pharaoh.Gameplay
{
    public class PullHookCapacity : HookCapacity
    {
        public float pullForce = 1f;
        public float pullMaxMovement = 1f;
        public float pullDuration = 1f;
        [SerializeField] private float pullOffset = 1f;

        [SerializeField, Tooltip("grapple movement curve from a to b")]
        public AnimationCurve smoothCurve;

        public UnityEvent<PullHookCapacity, PullHookBehaviour> onPullStart = new UnityEvent<PullHookCapacity, PullHookBehaviour>();

        public bool isPullPressed => _input.CharacterActions.Hook.IsPressed();
        
        private bool isBlocked 
        {
            get
            {
                if (!_currentTarget) return false;

                bool isFacingRight = _movement.isFacingRight;
                Vector2 direction = transform.position - _currentTarget.transform.position;
                if (direction.x >= 0.0f && !isFacingRight || direction.x <= 0.0f && isFacingRight)
                {
                    direction.x *= -1f;
                }

                var hits = Physics2D.RaycastAll(transform.position,
                    direction.normalized, direction.magnitude, whatIsObstacle);
                return hits.Length > 0 && hits[0].collider.gameObject == gameObject;
            }
        }

        public bool canPull
        {
            get
            {
                if (!_currentTarget) return false;
                Vector2 pos = transform.position;
                Vector2 targetPos = _currentTarget.transform.position;
                return targetPos.x > (pos.x + pullOffset) || targetPos.x < (pos.x - pullOffset);
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
            // test if something is between the block & the player
            // test if you can't pull the block 
            bool cantPullTarget = _currentTarget && !canPull;
            if (cantPullTarget || isBlocked) OnRelease();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            _movement.IsPullingBlock = false;
            _movement.IsHookedToBlock = false;
        }

        public void OnPullEnd(PullHookCapacity capacity, PullHookBehaviour behaviour)
        {
            if (capacity != this) return;
            OnRelease();
        }

        private void OnHook(InputAction.CallbackContext ctx)
        {
            // hook the nearest hookable objects if there is one
            if (!_potentialTarget) return;

            _currentTarget = _potentialTarget;

            if (!_currentTarget || !canPull) return;
            if (_movement.IsStunned || !_movement.isGrounded || _movement.IsPullingBlock ) return; 
            if (!_currentTarget.TryGetComponent(out PullHookBehaviour phb)) return; 
            
            Debug.Log($"Start pulling: {phb.name}");
            if (TryGetComponent(out Rigidbody2D rb))
            {
                rb.velocity = Vector2.zero;
            }

            _movement.IsPullingBlock = true;
            _movement.IsHookedToBlock = true;
            //_movement.IsFacingRight = _currentTarget.transform.position.x > transform.position.x;
            onPullStart?.Invoke(this, phb);
        }
        
        private void OnMove(InputAction.CallbackContext ctx)
        {
            if (!_currentTarget || !_movement.IsHookedToBlock) return;
            var axis = _input.CharacterControls.Move.ReadValue<Vector2>();

            if (Mathf.Abs(axis.x - Mathf.Epsilon) <= Mathf.Epsilon) return;
            OnRelease();
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (!_currentTarget || !_movement.IsHookedToBlock) return;
            OnRelease();
        }

        private void OnDash(InputAction.CallbackContext ctx)
        {
            if (!_currentTarget || !_movement.IsHookedToBlock) return;
            OnRelease();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_movement) return;

            if (!_movement.IsHookedToBlock)
            {
                // Draws the best target to the right (red if not the faced direction)
                Gizmos.color = _movement.isFacingRight
                    ? new Color(0f, 0.7531517f, 0f, 1f)
                    : new Color(0f, 0.7531517f, 0f, 0.1f);
                if (_bestTargetRight != null)
                    Gizmos.DrawLine(transform.position, _bestTargetRight.transform.position);

                // Draws the best target to the left (red if not the faced direction)
                Gizmos.color = !_movement.isFacingRight
                    ? new Color(0f, 0.7531517f, 0f, 1f)
                    : new Color(0f, 0.7531517f, 0f, 0.1f);
                if (_bestTargetLeft != null)
                    Gizmos.DrawLine(transform.position, _bestTargetLeft.transform.position);
            }
            else
            {
                if (!_currentTarget) return;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _currentTarget.transform.position);

                GUIStyle style = new GUIStyle();
                style.normal.textColor = new Color(0f, 0.5f, 0f);
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(_currentTarget.transform.position + Vector3.up * 3f, "| Pulling bounds |", style);

                Debug.DrawLine(_currentTarget.transform.position + new Vector3(1.5f, -3f, 0f), _currentTarget.transform.position + new Vector3(1.5f, 3f, 0f), new Color(0f, 0.5f, 0f));
                Debug.DrawLine(_currentTarget.transform.position + new Vector3(-1.5f, -3f, 0f), _currentTarget.transform.position + new Vector3(-1.5f, 3f, 0f), new Color(0f, 0.5f, 0f));
            }
        }
#endif
    }

}
