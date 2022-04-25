using System;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using MessageType = Pharaoh.Tools.Debug.MessageType;
using AudioManager = Pharaoh.Managers.AudioManager;
using InputFlags = InputReader.InputFlags;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pharaoh.Gameplay.Components.Movement
{
    [RequireComponent(typeof(Rigidbody2D))] //auto creates a Rigidbody2D component when attaching this component
    public class PlayerMovement : MonoBehaviour
    {
        private readonly Quaternion RightRotation = Quaternion.Euler(new Vector3(0f, 89.9f, 0f));
        private readonly Quaternion LeftRotationIdle = Quaternion.Euler(new Vector3(0f, -135f, 0f));
        private readonly Quaternion LeftRotationRunning = Quaternion.Euler(new Vector3(0f, -90.1f, 0f));

        private Rigidbody2D _rigidbody;
        private Collider2D _collider;
        private Vector2 _movementInput;
        private Vector2 _smoothMovement;
        private Quaternion _rotation = Quaternion.Euler(new Vector3(0f, 89.9f, 0f)); //used to compute the player model rotation
        private float _groundCheckLength = 0.05f;
        private float _previousGravityScale;
        private float _jumpClock = 0f; //used to measure for how long the jump input is held
        private float _dashClock = 0f; //used to measure for how long the dash occurs
        private float _smoothInput = 0.03f;
        private float _turnSpeed = 7f; //value defined with Cl?mence
        private int _defaultLayer;
        private int _dashLayer;
        private int _swarmDashLayer;
        private bool _isRunning = false;
        private bool _isDashing = false;
        private bool _hasDashedInAir = false;
        private bool _isJumping = false;
        private bool _noclip; //DEBUG
        private bool _canMove = true;
        private bool _isHooked = false;
        private bool _isPullingBlock = false;
        //true when the grappling is running, false otherwise
        private bool _isHooking = false;
        //true when the trigger has already been set during this coroutine, false otherwise
        private bool _willBeHooked = false;
        private bool _isFacingRight = true;
        private bool _isGrounded = false;

        public bool IsFacingRight => _isFacingRight;
        public bool IsGrounded => _isGrounded;
        public bool IsRunning => _isRunning;
        public bool IsDashing => _isDashing;
        public bool IsJumping => _isJumping;
        public bool IsPullingBlock => _isPullingBlock;
        public bool IsHooking => _isHooking;

        [Header("Input Reader")]
        public InputReader inputReader;

        [Header("Player Movement Data")]
        public PlayerMovementData metrics;

        [Header("Player Skills Unlocked")]
        public PlayerSkills skills;

        [SerializeField, Header("Hook Events")]
        private HookBehaviourEvents hookEvents;

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

        [Header("VFX")]
        [Tooltip("VFX for the swarm dash")]
        public VisualEffect swarmDashVFX;

        [Header("Dash Detection")]
        [SerializeField] private LayerMask dashStunLayer;
        [SerializeField] private StunData dashStunData;
        [SerializeField] private UnityEvent<GameObject, StunData> onDashStun;
        [SerializeField] private UnityEvent onDashEnd;

        private bool _canJumpHook;

        private void Awake()
        {
            _defaultLayer = LayerMask.NameToLayer("Player");
            _dashLayer = LayerMask.NameToLayer("Player - Dash");
            _swarmDashLayer = LayerMask.NameToLayer("Player - Swarm");
            _rigidbody = GetComponent<Rigidbody2D>();
            inputReader.Initialize(); //need to manually initialize
            if (metrics) _rigidbody.gravityScale = metrics.gravityScale;

            if (!TryGetComponent(out _collider))
            {
                LogHandler.SendMessage($"No collider on the player", MessageType.Warning);
            }
        }

        private void OnEnable()
        {
            HookAddListener();

            inputReader.movePerformedEvent += OnMove;
            inputReader.moveCanceledEvent += OnMove;

            inputReader.jumpStartedEvent += OnJumpStarted;
            inputReader.jumpCanceledEvent += OnJumpCanceled;

            inputReader.dashStartedEvent += OnDashStarted;

            inputReader.noclipPerformedEvent += OnNoclipPerformed;
        }

        private void OnDisable()
        {
            HookRemoveListener();

            inputReader.movePerformedEvent -= OnMove;
            inputReader.moveCanceledEvent -= OnMove;

            inputReader.jumpStartedEvent -= OnJumpStarted;
            inputReader.jumpCanceledEvent -= OnJumpCanceled;

            inputReader.dashStartedEvent -= OnDashStarted;

            inputReader.noclipPerformedEvent -= OnNoclipPerformed;
        }

        public void OnMove(Vector2 mvt)
        {
            // Recover the player's input, clamping it to avoid diagonals directions
            _movementInput = Vector2.zero;
            if (!_noclip)
            {
                if (mvt.x >= 0.2f)
                    _movementInput = Vector2.right;
                else if (mvt.x <= -0.2f)
                    _movementInput = Vector2.left;
            }
            else
                _movementInput = mvt;

            if (_movementInput.y < -0.8f) _isHooked = false;
        }

        // Triggers when the player jumps
        private void OnJumpStarted()
        {
            if ((_isGrounded || _isHooked || _canJumpHook) && !_isDashing && !_isPullingBlock)
            {
                // The player jumps using an impulse force
                _rigidbody.AddForce(Vector2.up * (metrics.initialJumpForce * _rigidbody.mass), ForceMode2D.Impulse);
                _rigidbody.gravityScale = metrics.gravityScale;
                _jumpClock = Time.time;
                _isJumping = true;
                _canJumpHook = false;

                animator.SetTrigger("Jumping");
            }
        }

        // Triggers when the player stops jumping
        private void OnJumpCanceled()
        {
            // The player released the jump button
            _isJumping = false;
        }

        // Triggers when the player dashes
        private void OnDashStarted()
        {
            if (skills.HasDash && !_isDashing && !_hasDashedInAir && !_isPullingBlock)
            {
                _rigidbody.velocity = Vector2.zero;
                
                _rigidbody.gravityScale = 0f;

                _dashClock = Time.time;
                _isDashing = true;
                inputReader.DisableInputs(InputFlags.Dash);
                _isHooked = false;

                gameObject.layer = _dashLayer;

                if (skills.HasSwarmDash)
                {
                    gameObject.layer = _swarmDashLayer;
                    foreach (Renderer r in GetComponentsInChildren<Renderer>()) r.enabled = false;
                    swarmDashVFX.SetVector3("StartPosition", transform.position);
                    swarmDashVFX.SetBool("IsFacingRight", _isFacingRight);
                    swarmDashVFX.enabled = true;
                    AudioManager.Instance.Play("DashSwarm");
                } 
                else
                {
                    AudioManager.Instance.Play("DashNormal");
                }

                animator.SetTrigger("Dashing");

                StartCoroutine(OverlapStunable());
            }
        }

        // DEBUG
        private void OnNoclipPerformed()
        {
            _rigidbody.gravityScale = (_noclip = !_noclip) ? 0f : metrics.gravityScale;
        }

        public void LockMovement(bool value)
        {
            _canMove = !value;
        }

        #region Hook

        private void HookAddListener()
        {
            if (!hookEvents) return;
            hookEvents.started += OnHookStarted;
            hookEvents.performed += OnHookPerformed;
            hookEvents.ended += OnHookEnded;
            hookEvents.released += OnHookReleased;
        }

        private void HookRemoveListener()
        {
            if (!hookEvents) return;
            hookEvents.started -= OnHookStarted;
            hookEvents.performed -= OnHookPerformed;
            hookEvents.ended -= OnHookEnded;
            hookEvents.released -= OnHookReleased;
        }

        private void OnHookStarted(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            LockMovement(true);

            switch (behaviour)
            {
                case GrappleHookBehaviour grapple:
                    animator.SetTrigger("Hooking"); //sends a signal that the player has started the hooking process
                    _isHooking = true; //tells the PlayerMovement that the player is in a state of hooking
                    break;
                case PullHookBehaviour pull:
                    _rigidbody.velocity = Vector2.zero;
                    _isPullingBlock = true;
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
                    // When reaching a 90% threshold of travelled distance,
                    // sends a signal that the player'll soon be hooked
                    if (grapple.travelPercent >= 0.9f && !_willBeHooked)
                    {
                        animator.SetTrigger("Will Be Hooked");
                        _willBeHooked = true;
                    }
                    // center the body with the collider center
                    var target = grapple.nextPosition - _collider.offset;
                    _rigidbody.MovePosition(target);
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
                    _willBeHooked = false; //reuse this variable each time we grapple
                    _isHooking = false; //tells the PlayerMovement that the player finished hooking
                    inputReader.EnableInputs(InputFlags.Jump);
                    _isHooked = true;
                    _canJumpHook = true;
                    _hasDashedInAir = false;
                    animator.SetBool("Is Grounded", _isGrounded);
                    _rigidbody.velocity = Vector2.zero;
                    _rigidbody.gravityScale = 0f;
                    break;
                case PullHookBehaviour pull:
                    LockMovement(false);
                    _isPullingBlock = false;
                    break;
                case SnatchHookBehaviour snatch:
                    LockMovement(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour));
            }
        }

        private void OnHookReleased(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            LockMovement(false);
            _isHooked = false;

            switch (behaviour)
            {
                case GrappleHookBehaviour grapple:
                    animator.ResetTrigger("Will Be Hooked"); //resets the trigger to avoid remnants when unhooking
                    if (_isHooking) //the player cancelled the action while hooking, ie. while not yet hooked
                    {
                        _isHooking = false; //the player is no longer hooking
                        animator.SetTrigger("Hooking Cancelled"); //sends a signal that the player cancelled the hooking process
                    }
                    _willBeHooked = false; //reuse this variable each time we grapple
                    _rigidbody.gravityScale = metrics.gravityScale; // reset the base gravity of the player
                    break;
                case PullHookBehaviour pull:
                    _isPullingBlock = false;
                    break;
                case SnatchHookBehaviour snatch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour));
            }
        }
        
        #endregion

        private void Update()
        {
            Vector2 vel = Vector2.zero; //useless but necessary for the SmoothDamp
            _smoothMovement = Vector2.SmoothDamp(_smoothMovement, _movementInput, ref vel, _smoothInput);

            // Stops the jump if held for too long
            if (_isJumping && _jumpClock + metrics.maxJumpHold < Time.time)
                _isJumping = false;

            // Stops the dash when its duration is past
            if (_isDashing && _dashClock + metrics.dashDuration < Time.time)
            {
                _rigidbody.velocity = Vector2.zero;
                _rigidbody.gravityScale = metrics.gravityScale;
                _isDashing = false;
                onDashEnd?.Invoke();

                gameObject.layer = _defaultLayer;

                foreach (Renderer r in GetComponentsInChildren<Renderer>()) r.enabled = true;
                swarmDashVFX.enabled = false;

                StartCoroutine(DashCooldown());
            }

            RotatePlayerModel(); //don't read this.. or at least don't try to understand it -> it just works
            UpdateStates();
        }

        private void FixedUpdate()
        {
            // remove jump when releasing from hook
            if (_canJumpHook && _rigidbody.velocity.y <= -Mathf.Epsilon)
            {
                _canJumpHook = false;
            }

            // Moves the player horizontally with according speeds while not dashing
            if (!_isDashing && _canMove && !_isPullingBlock)
            {
                if (_isGrounded || _isHooked)
                    _rigidbody.velocity = new Vector2(_smoothMovement.x * metrics.horizontalSpeed, _rigidbody.velocity.y);
                else
                    _rigidbody.velocity = new Vector2(_smoothMovement.x * metrics.inAirHorizontalSpeed, _rigidbody.velocity.y);
            }

            // DEBUG
            if (_noclip)
                _rigidbody.velocity = _movementInput * metrics.noclipSpeed;

            // Moves the player upward while holding the jump button
            if (_isJumping)
                _rigidbody.AddForce(Vector2.up * (metrics.heldJumpForce * _rigidbody.mass), ForceMode2D.Force);

            if (_isDashing)
                _rigidbody.velocity = (_isFacingRight ? Vector2.right : Vector2.left) * metrics.dashForce;
        }

        private void LateUpdate()
        {
            animator.SetFloat("Vertical Velocity", _rigidbody.velocity.y);
            animator.SetFloat("Horizontal Velocity", _rigidbody.velocity.x);
        }

        private void UpdateStates()
        {
            // Limit the dash to one use per air-time
            if (_hasDashedInAir && _isGrounded)
                _hasDashedInAir = false;

            // Updates the direction the player is facing
            if (_smoothMovement.x != 0f && !_isPullingBlock && !_isHooking)
                _isFacingRight = Mathf.Sign(_smoothMovement.x) == 1f;

            // Updates whether the player is running or not
            if (_smoothMovement.x != 0f && Mathf.Abs(_rigidbody.velocity.x) > 0.01f) _isRunning = true;
            else _isRunning = false;
            
            bool wasGrounded = _isGrounded;

            // Updates the grounded state - check if one or both "feet" are on a ground
            _isGrounded = Physics2D.Raycast(rightGroundCheck.position, Vector2.down, _groundCheckLength, groundLayer)
                || Physics2D.Raycast(leftGroundCheck.position, Vector2.down, _groundCheckLength, groundLayer);

            // only when reach the ground and not falling
            _isGrounded = _isGrounded && _rigidbody.velocity.y <= Mathf.Epsilon && _rigidbody.velocity.y >= -Mathf.Epsilon;

            // Updates the grounded state - check if one or both "feet" are on a ground
            _isGrounded = Physics2D.Raycast(rightGroundCheck.position, Vector2.down, _groundCheckLength, groundLayer)
                          || Physics2D.Raycast(leftGroundCheck.position, Vector2.down, _groundCheckLength, groundLayer);
            
            animator.SetBool("Is Grounded", _isGrounded);
            animator.SetBool("Is Facing Right", _isFacingRight);
            animator.SetBool("Is Pulling", _isPullingBlock);
            animator.SetBool("Is Hooked", _isHooked);
        }

        // Coroutine re-enabling the dash after it's cooldown
        private System.Collections.IEnumerator DashCooldown()
        {
            onDashStun?.Invoke(null, null);

            yield return new WaitForSeconds(metrics.dashCooldown);

            inputReader.EnableInputs(InputFlags.Dash);
        }

        private System.Collections.IEnumerator OverlapStunable()
        {
            if (!_rigidbody) yield break;

            int size = 0;
            Collider2D[] colls = new Collider2D[5];
            var boxSize = new Vector2(1, 2);

            while (_isDashing)
            {
                size = Physics2D.OverlapBoxNonAlloc(_rigidbody.position, boxSize, 0f, colls, dashStunLayer);

                if (size <= 0) yield return new WaitForFixedUpdate();

                foreach (var col in colls)
                {
                    if (!col || !col.gameObject) continue;
                    LogHandler.SendMessage($"{name} found {col.name} while dashing", MessageType.Log);
                    onDashStun?.Invoke(col.gameObject, dashStunData);
                }

                yield return new WaitForFixedUpdate();
            }
        }

        public void Stun(float duration)
        {
            _rigidbody.velocity = Vector2.zero;

            inputReader.DisableInputs(InputFlags.All);

            StartCoroutine(Stunned(duration));
            animator.SetTrigger("Stunned");
        }

        // Coroutine for the duration of the stun
        System.Collections.IEnumerator Stunned(float duration)
        {
            yield return new WaitForSeconds(duration);
            inputReader.EnableInputs(InputFlags.All);
        }

        public void Respawn()
        {
            _isRunning = false;
            _isDashing = false;
            _hasDashedInAir = false;
            _isJumping = false;
            _noclip = false; //DEBUG
            _isHooked = false;
            _isPullingBlock = false;
            _isHooking = false;
            LockMovement(false);

            _jumpClock = 0f;
            _dashClock = 0f;

            _rigidbody.gravityScale = metrics.gravityScale;

            gameObject.layer = _defaultLayer;

            Stun(metrics.respawnStunDuration);
        }

        // Turns the character model around when facing the other direction
        private void RotatePlayerModel()
        {
            Quaternion from = _rotation;
            // Lerps between a given orientation when idle facing left and when running facing left
            // This is used because facing left would normally put the back of the model towards the camera -> not fancy !!
            Quaternion to = (_rigidbody.velocity.x > 2f || _rigidbody.velocity.x < -2f) || _isDashing || _isPullingBlock || _isHooking ?
                    Quaternion.Lerp(_isFacingRight ? RightRotation : LeftRotationRunning, _isFacingRight ? RightRotation : LeftRotationIdle, 0f)
                    : Quaternion.Lerp(_isFacingRight ? RightRotation : LeftRotationIdle, _isFacingRight ? RightRotation : LeftRotationRunning, 0f);
            _rotation = Quaternion.Lerp(from, to, Time.deltaTime * _turnSpeed);

            // The previously computed rotation is used only when the player isn't hooked, when he is the animator turns it himself
            if (!_isHooked)
                modelTransform.localRotation = _rotation;
            else
                modelTransform.localRotation = Quaternion.Euler(new Vector3(0f, 89.9f, 0f));
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
            Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + _rigidbody.velocity.normalized * 0.05f);
            //Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + _rigidbody.velocity);

            // Displays stats on top of the player
            Handles.Label(_rigidbody.position + Vector2.up * 4.2f, "IsPullingBlock : " + _isPullingBlock, _isPullingBlock ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 4.0f, "IsHooked : " + _isHooked, _isHooked ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 3.8f, "IsPullingBlock : " + _isPullingBlock, _isPullingBlock ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 3.6f, "IsJumping : " + _isJumping, _isJumping ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 3.4f, "IsDashing : " + _isDashing, _isDashing ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 3.2f, "HasDashedInAir : " + _hasDashedInAir, _hasDashedInAir ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2.8f, "IsRunning : " + _isRunning, _isRunning ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2.6f, "IsFacingRight : " + _isFacingRight, _isFacingRight ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2.4f, "isFalling : " + _isGrounded, _isGrounded ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2.2f, "Speed : " + _rigidbody?.velocity.magnitude + " m/s", _rigidbody.velocity.magnitude != 0f ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 2f, "NOCLIP (O) : " + _noclip, _noclip ? greenStyle : redStyle);
            Handles.Label(_rigidbody.position + Vector2.up * 1.8f, "" + inputReader);
        }
#endif
    }
    
}
