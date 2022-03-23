using System;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pharaoh.Gameplay.Components.Movement
{
    [RequireComponent(typeof(Rigidbody2D))] //auto creates a Rigidbody2D component when attaching this component
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        private Vector2 _movementInput;
        private Vector2 _smoothMovement;
        private PlayerInput _playerInput;
        private float _groundCheckLength = 0.05f;
        private float _previousGravityScale;
        private float _jumpClock = 0f; //used to measure for how long the jump input is held
        private float _dashClock = 0f; //used to measure for how long the dash occurs
        private float _smoothInput = 0.03f;
        private float _turnSpeed = 7f; //value defined with Cl?mence
        private float _backOrientationIdle = -135f; //value defined with Cl?mence
        private float _backOrientationRunning = -90.1f; //value defined with Cl?mence
        private float _initialFallHeight;
        private int _defaultLayer;
        private int _swarmDashLayer;
        private bool _isRunning = false;
        private bool _isDashing = false;
        private bool _hasDashedInAir = false;
        private bool _isDashAvailable = true;
        private bool _isJumping = false;
        private bool _isStunned = false;
        private bool _noclip; //DEBUG
        private bool _canMove = true;
        private bool _isHooked = false;
        private bool _isHookedToBlock = false;
        private bool _isPullingBlock = false;

        public bool isFacingRight { get; private set; } = true;
        public bool isGrounded { get; private set; } = false;

        public bool IsRunning { get => _isRunning; }
        public bool IsStunned { get => _isStunned; }
        public bool IsDashing { get => _isDashing; }
        public bool IsJumping { get => _isJumping; }
        public bool IsFacingRight { get => isFacingRight; set => isFacingRight = value; }
        public bool IsGrounded { get => isGrounded; }
        public bool IsHookedToBlock {
            get => _isHookedToBlock;
            set { _isHookedToBlock = value;
                if (_isHookedToBlock) { _smoothMovement = Vector2.zero; _isRunning = false; }
            }
        }
        public bool IsPullingBlock { get => _isPullingBlock; set => _isPullingBlock = value; }

        [Header("Horizontal Movement")]
        [Tooltip("Grounded horizontal speed (m/s)")]
        public float horizontalSpeed = 5f;
        [Tooltip("In-air horizontal speed (m/s)")]
        public float inAirHorizontalSpeed = 5f;
        [Tooltip("NOCLIP mode speed (m/s)")]
        public float noclipSpeed = 10f;
        [Tooltip("How long the player is stunned when getting damaged")]
        public float respawnStunDuration = 1.5f;

        [Header("Jump")]
        [Tooltip("Defines the force added to the player when initiating the jump")]
        public float initialJumpForce = 12f;
        [Tooltip("Defines the force added to the player while holding the jump button")]
        public float heldJumpForce = 16f;
        [Tooltip("How long the player can hold down the jump button after jumping")]
        public float maxJumpHold = 0.3f;
        [Tooltip("How high the fall has to be to stun the player")]
        public float stunFallDistance = 1.85f * 3f;
        [Tooltip("How long the player is stunned when falling from too high")]
        public float fallStunDuration = 1.5f;

        [Header("Dash")]
        [Tooltip("Dashing force, 50 works well")]
        public float dashForce = 30f;
        [Tooltip("Dashing time, 0.1 works well")]
        public float dashTime = 0.1f;
        [Tooltip("Dashing time, 0.1 works well")]
        public float dashDuration = 0.1f;
        [Tooltip("Cooldown between each dash, starts at the end of the previous one")]
        public float dashCooldown = 0.5f;

        [Header("Ground Detection")]
        [Tooltip("Rightmost ground check")]
        public Transform rightGroundCheck;
        [Tooltip("leftmost ground check")]
        public Transform leftGroundCheck;
        public LayerMask groundLayer;

        [Header("Animations")]
        [Tooltip("Animator controlling the player")]
        public Animator animator;
        [Tooltip("Model transform to turn the player around")]
        public Transform modelTransform;

        [Header("DEBUG")]
        public TrailRenderer tr; //DEBUG

        private void Awake()
        {
            _defaultLayer = LayerMask.NameToLayer("Player");
            _swarmDashLayer = LayerMask.NameToLayer("Player - Swarm");
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerInput = new PlayerInput();
        }

        private void OnEnable()
        {
            // Hook bindings
            HookBehaviour.started += OnHookStarted;
            HookBehaviour.performed += OnHookPerformed;
            HookBehaviour.ended += OnHookEnded;
            HookBehaviour.released += OnHookReleased;

            // Movement's events binding
            _playerInput.Enable();

            _playerInput.CharacterControls.Move.performed += OnMove; //player starts moving
            _playerInput.CharacterControls.Move.canceled += OnMove; //player ends moving
            _playerInput.CharacterControls.Jump.started += OnJump; //player starts jumping
            _playerInput.CharacterControls.Jump.canceled += OnJump; //player ends jumping
            _playerInput.CharacterControls.Dash.started += OnDash; //player dashes

            _playerInput.CharacterControls.NOCLIP.performed += OnNoclip; //enter NOCLIP mode
        }

        private void OnDisable()
        {
            // Hook bindings
            HookBehaviour.started -= OnHookStarted;
            HookBehaviour.performed -= OnHookPerformed;
            HookBehaviour.ended -= OnHookEnded;
            HookBehaviour.released -= OnHookReleased;

            // Movement's events binding
            _playerInput.CharacterControls.Move.performed -= OnMove; //player starts moving
            _playerInput.CharacterControls.Move.canceled -= OnMove; //player ends moving
            _playerInput.CharacterControls.Jump.started -= OnJump; //player starts jumping
            _playerInput.CharacterControls.Jump.canceled -= OnJump; //player ends jumping
            _playerInput.CharacterControls.Dash.started -= OnDash; //player dashes

            _playerInput.CharacterControls.NOCLIP.performed -= OnNoclip; //enter NOCLIP mode

            _playerInput.Disable();
        }

        // Triggers when the Move input is triggered or released, modifies the movement input vector according to player controls
        private void OnMove(InputAction.CallbackContext ctx)
        {
            // Recover the player's input
            _movementInput = _playerInput.CharacterControls.Move.ReadValue<Vector2>();

            if (_movementInput.y < -0.8f) _isHooked = false;
        }

        // Triggers when the player jumps
        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.started && (isGrounded || _isHooked) && !_isDashing && !_isStunned && !_isPullingBlock)
            {
                // The player jumps using an impulse force
                _rigidbody.AddForce(Vector2.up * initialJumpForce, ForceMode2D.Impulse);
                _jumpClock = Time.time;
                _isJumping = true;
                _isHooked = false;

                animator.SetTrigger("Jumping");
            }
            else if (ctx.canceled)
            {
                // The player released the jump button
                _isJumping = false;
            }
        }

        // Triggers when the player dashes
        private void OnDash(InputAction.CallbackContext ctx)
        {
            if (!_isDashing && _isDashAvailable && !_hasDashedInAir && !_isStunned && !_isPullingBlock)
            {
                _rigidbody.velocity = Vector2.zero;

                _previousGravityScale = _rigidbody.gravityScale;
                _rigidbody.gravityScale = 0f;

                _dashClock = Time.time;
                _isDashing = true;
                _isDashAvailable = false;
                _isHooked = false;

                gameObject.layer = _swarmDashLayer;

                animator.SetTrigger("Dashing");
            }
        }

        // DEBUG
        private void OnNoclip(InputAction.CallbackContext ctx)
        {
            _noclip = !_noclip;

            if (_noclip)
                _rigidbody.gravityScale = 0f;
            else
                _rigidbody.gravityScale = 3f;
        }

        public void LockMovement(bool value)
        {
            _canMove = !value;
        }

        private void OnHookStarted(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            LockMovement(true);

            switch (behaviour)
            {
                case GrappleHookBehaviour grapple:
                    //animator?.SetTrigger(Animator.StringToHash("grapple_start"));
                    break;
                case PullHookBehaviour pull:
                    _rigidbody.velocity = Vector2.zero;
                    IsPullingBlock = true;
                    IsHookedToBlock = true;
                    break;
                case SnatchHookBehaviour snatch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour));
            }
        }

        private void OnHookPerformed(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            switch (behaviour)
            {
                case GrappleHookBehaviour grapple:
                    _rigidbody.MovePosition(grapple.nextPosition);
                    break;
                case PullHookBehaviour pull:
                    break;
                case SnatchHookBehaviour snatch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour));
            }
        }

        private void OnHookEnded(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            switch (behaviour)
            {
                case GrappleHookBehaviour grapple:
                    //animator?.SetTrigger(Animator.StringToHash("grapple_end"));
                    _initialFallHeight = _rigidbody.position.y;
                    _isHooked = true;
                    _hasDashedInAir = false;
                    animator.SetBool("Is Grounded", isGrounded);
                    _initialFallHeight = _rigidbody.position.y;
                    _rigidbody.velocity = Vector2.zero;
                    _rigidbody.bodyType = RigidbodyType2D.Kinematic;
                    break;
                case PullHookBehaviour pull:
                    IsPullingBlock = false;
                    IsHookedToBlock = false;
                    break;
                case SnatchHookBehaviour snatch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour));
            }
        }

        private void OnHookReleased(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            LockMovement(false);

            switch (behaviour)
            {
                case GrappleHookBehaviour grapple:
                    _rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    break;
                case PullHookBehaviour pull:
                    IsPullingBlock = false;
                    IsHookedToBlock = false;
                    break;
                case SnatchHookBehaviour snatch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour));
            }
        }

        private void Update()
        {
            Vector2 vel = Vector2.zero; //useless but necessary for the SmoothDamp
            _smoothMovement = Vector2.SmoothDamp(_smoothMovement, _movementInput, ref vel, _smoothInput);

            // Stops the jump if held for too long
            if (_isJumping && _jumpClock + maxJumpHold < Time.time)
                _isJumping = false;

            // Stops the dash when its duration is past
            if (_isDashing && _dashClock + dashDuration < Time.time)
            {
                _rigidbody.velocity = Vector2.zero;
                _rigidbody.gravityScale = _previousGravityScale;
                _isDashing = false;

                gameObject.layer = _defaultLayer;

                StartCoroutine(DashCooldown());
            }

            // Turns the character model around when facing the other direction
            Quaternion from = modelTransform.localRotation;
            Quaternion toIdle = isFacingRight ? Quaternion.Euler(new Vector3(0f, 89.9f, 0f)) : Quaternion.Euler(new Vector3(0f, _backOrientationIdle, 0f));
            Quaternion toRunning = isFacingRight ? Quaternion.Euler(new Vector3(0f, 89.9f, 0f)) : Quaternion.Euler(new Vector3(0f, _backOrientationRunning, 0f));
            // Lerps between a given orientation when idle facing left and when running facing left
            // This is used because facing left would normally put the back of the model towards the camera -> not fancy !!
            Quaternion to = (_movementInput.x == 0f && !_isDashing) || _isStunned || _isPullingBlock ?
                Quaternion.Lerp(toIdle, toRunning, 0f)
                : Quaternion.Lerp(toRunning, toIdle, 0f);
            modelTransform.localRotation = Quaternion.Lerp(from, to, Time.deltaTime * _turnSpeed);

            UpdateStates();
        }

        private void FixedUpdate()
        {
            // Moves the player horizontally with according speeds while not dashing
            if (!_isDashing && _canMove && !_isStunned && !_isPullingBlock)
            {
                if (isGrounded || _isHooked)
                    _rigidbody.velocity = new Vector2(_smoothMovement.x * horizontalSpeed, _rigidbody.velocity.y);
                else
                    _rigidbody.velocity = new Vector2(_smoothMovement.x * inAirHorizontalSpeed, _rigidbody.velocity.y);
            }

            // DEBUG
            if (_noclip)
                _rigidbody.velocity = _movementInput * noclipSpeed;

            // Moves the player upward while holding the jump button
            if (_isJumping && !_isStunned)
                _rigidbody.AddForce(Vector2.up * heldJumpForce, ForceMode2D.Force);

            if (_isDashing && !_isStunned)
                _rigidbody.velocity = (IsFacingRight ? Vector2.right : Vector2.left) * dashForce;
        }

        private void LateUpdate()
        {
            animator.SetFloat("Vertical Velocity", _rigidbody.velocity.y);
            animator.SetFloat("Horizontal Velocity", _rigidbody.velocity.x);
        }

        private void UpdateStates()
        {
            // Limit the dash to one use per air-time
            if (_hasDashedInAir && isGrounded)
                _hasDashedInAir = false;

            // Updates the direction the player is facing
            if (_smoothMovement.x != 0f && !_isStunned && !_isHookedToBlock && !_isPullingBlock)
            {
                isFacingRight = Mathf.Sign(_smoothMovement.x) == 1f;
            }

            // Updates whether the player is running or not
            if (_smoothMovement.x != 0f && Mathf.Abs(_rigidbody.velocity.x) > 0.01f) _isRunning = true;
            else _isRunning = false;
            
            bool wasGrounded = isGrounded;

            // Updates the grounded state - check if one or both "feet" are on a ground
            isGrounded = Physics2D.Raycast(rightGroundCheck.position, Vector2.down, _groundCheckLength, groundLayer)
                || Physics2D.Raycast(leftGroundCheck.position, Vector2.down, _groundCheckLength, groundLayer);

            // only when reach the ground and not falling
            isGrounded = isGrounded && _rigidbody.velocity.y <= Mathf.Epsilon && _rigidbody.velocity.y >= -Mathf.Epsilon;

            // Updates the grounded state - check if one or both "feet" are on a ground
            isGrounded = Physics2D.Raycast(rightGroundCheck.position, Vector2.down, _groundCheckLength, groundLayer)
                || Physics2D.Raycast(leftGroundCheck.position, Vector2.down, _groundCheckLength, groundLayer);
            
            animator.SetBool("Is Grounded", isGrounded);

            // Updates the in-air distance traveled and stuns if necessary
            if (wasGrounded != isGrounded && wasGrounded)
            {
                // The player leaves the ground
                _initialFallHeight = _rigidbody.position.y;
            }
            else if (wasGrounded != isGrounded && !wasGrounded)
            {
                // Player reached the ground
                
                if (_initialFallHeight - _rigidbody.position.y > stunFallDistance)
                {
                    // Player fell from too high -> Stun
                    Stun(fallStunDuration);
                }
            }
        }

        // Coroutine for the duration of the dash
        System.Collections.IEnumerator Dashing()
        {
            yield return new WaitForSeconds(dashTime);

            // Re-enables gravity on the player
            _rigidbody.gravityScale = _previousGravityScale;

            // Updates current state
            _isDashing = false;

            tr.startColor = Color.blue; //DEBUG

            StartCoroutine(DashCooldown());
        }

        // Coroutine re-enabling the dash after it's cooldown
        System.Collections.IEnumerator DashCooldown()
        {
            yield return new WaitForSeconds(dashCooldown);

            _isDashAvailable = true;
        }

        public void Stun(float duration)
        {
            _rigidbody.velocity = Vector2.zero;
            _isStunned = true;
            StartCoroutine(Stunned(duration));
            animator.SetTrigger("Stunned");
        }

        // Coroutine for the duration of the stun
        System.Collections.IEnumerator Stunned(float duration)
        {
            yield return new WaitForSeconds(duration);

            // Updates current state
            _isStunned = false;
        }

        public void Respawn()
        {
             _isRunning = false;
             _isDashing = false;
             _hasDashedInAir = false;
             _isDashAvailable = true;
             _isJumping = false;
             _noclip = false; //DEBUG
             _canMove = true;
             _isHooked = false;
             _isHookedToBlock = false;
             _isPullingBlock = false;

            _jumpClock = 0f;
            _dashClock = 0f;
            _initialFallHeight = _rigidbody.position.y;

            _rigidbody.gravityScale = 3f;

            gameObject.layer = _defaultLayer;

            Stun(respawnStunDuration);
        }

    #if UNITY_EDITOR
        // Displays a bunch of stats while the game is playing
        private void OnDrawGizmosSelected()
        {
            if (_rigidbody == null) return;

            // Styles used to display stats
            GUIStyle redStyle = new GUIStyle();
            GUIStyle greenStyle = new GUIStyle();
            redStyle.normal.textColor = Color.red;
            greenStyle.normal.textColor = Color.green;

            // Displays the ground checks radiuses
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(rightGroundCheck.position, rightGroundCheck.position + (Vector3.down * _groundCheckLength));
            Gizmos.DrawLine(leftGroundCheck.position, leftGroundCheck.position + (Vector3.down * _groundCheckLength));

            // Displays the velocity
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + _rigidbody.velocity);

            // Displays stats on top of the player
            Handles.Label(_rigidbody.position + Vector2.up * 4.6f, "IsPullingBlock : " + _isPullingBlock, _isPullingBlock ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 4.4f, "IsHooked : " + _isHooked, _isHooked ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 4.2f, "IsHookedToBlock : " + _isHookedToBlock, _isHookedToBlock ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 4f, "FallDistance : " + (_initialFallHeight - _rigidbody.position.y), (_initialFallHeight - _rigidbody.position.y) > stunFallDistance ? redStyle : greenStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 3.8f, "IsStunned : " + _isStunned, _isStunned ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 3.6f, "IsJumping : " + _isJumping, _isJumping ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 3.4f, "IsDashing : " + _isDashing, _isDashing ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 3.2f, "HasDashedInAir : " + _hasDashedInAir, _hasDashedInAir ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 3f, "IsDashAvailable : " + _isDashAvailable, _isDashAvailable ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2.8f, "IsRunning : " + _isRunning, _isRunning ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2.6f, "IsFacingRight : " + isFacingRight, isFacingRight ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2.4f, "isFalling : " + isGrounded, isGrounded ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2.2f, "Speed : " + _rigidbody?.velocity.magnitude + " m/s", _rigidbody.velocity.magnitude != 0f ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2f, "NOCLIP (O) : " + _noclip, _noclip ? greenStyle : redStyle);
        }
    #endif
    }
    
}
