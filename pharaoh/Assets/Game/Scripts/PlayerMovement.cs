using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(Rigidbody2D))] //auto creates a Rigidbody2D component when attaching this component
public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothMovement;
    private PlayerInput _playerInput;
    private float _previousGravityScale;
    private float _jumpClock = 0f; //used to measure for how long the jump input is held
    private float _smoothInput = 0.03f;
    private float _turnSpeed = 7f; //value defined with Clémence
    private float _backOrientationIdle = -135f; //value defined with Clémence
    private float _backOrientationRunning = -90.1f; //value defined with Clémence
    private bool _isGrounded = false;
    private bool _isRunning = false;
    private bool _isFacingRight = true;
    private bool _isDashing = false;
    private bool _hasDashedInAir = false;
    private bool _isDashAvailable = true;
    private bool _isJumping = false;
    private bool _noclip; //DEBUG
    private bool _canMove = true;

    public bool IsFacingRight { get => _isFacingRight; }

    [Header("Horizontal Movement")]
    [Tooltip("Grounded horizontal speed (m/s)")]
    public float horizontalSpeed = 5f;
    [Tooltip("In-air horizontal speed (m/s)")]
    public float inAirHorizontalSpeed = 5f;
    [Tooltip("NOCLIP mode speed (m/s)")]
    public float noclipSpeed = 10f;

    [Header("Jump")]
    [Tooltip("Defines the force added to the player when initiating the jump")]
    public float initialJumpForce = 12f;
    [Tooltip("Defines the force added to the player while holding the jump button")]
    public float heldJumpForce = 16f;
    [Tooltip("How long the player can hold down the jump button after jumping")]
    public float maxJumpHold = 0.3f;

    [Header("Dash")]
    [Tooltip("Dashing force, 50 works well")]
    public float dashForce = 30f;
    [Tooltip("Dashing time, 0.1 works well")]
    public float dashTime = 0.1f;
    [Tooltip("Cooldown between each dash, starts at the end of the previous one")]
    public float dashCooldown = 0.5f;

    [Header("Ground Detection")]
    [Tooltip("Rightmost ground check")]
    public Transform rightGroundCheck;
    [Tooltip("Leftmost ground check")]
    public Transform leftGroundCheck;
    [Tooltip("0.05 to get a fine ground detection, keep it small and precise")]
    public float groundCheckRadius = 0.05f;
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
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInput = new PlayerInput();

        // Movement's events binding
        _playerInput.CharacterControls.Move.performed += OnMove; //player starts moving
        _playerInput.CharacterControls.Move.canceled += OnMove; //player ends moving
        _playerInput.CharacterControls.Jump.started += OnJump; //player starts jumping
        _playerInput.CharacterControls.Jump.canceled += OnJump; //player ends jumping
        _playerInput.CharacterControls.Dash.started += OnDash; //player dashes

        _playerInput.CharacterControls.NOCLIP.performed += OnNoclip; //enter NOCLIP mode
    }

    // Triggers when the Move input is triggered or released, modifies the movement input vector according to player controls
    private void OnMove(InputAction.CallbackContext ctx)
    {
        // Recover the player's input
        _movementInput = _playerInput.CharacterControls.Move.ReadValue<Vector2>();
    }

    // Triggers when the player jumps
    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started && _isGrounded && !_isDashing)
        {
            // The player jumps using an impulse force
            _rigidbody.AddForce(Vector2.up * initialJumpForce, ForceMode2D.Impulse);
            _jumpClock = Time.time;
            _isJumping = true;

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
        if (!_isDashing && _isDashAvailable && !_hasDashedInAir)
        {
            // Resets the velocity and adds the dash force towards facing direction
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.AddForce((_isFacingRight ? Vector2.right : Vector2.left) * dashForce, ForceMode2D.Impulse);

            // Disables gravity while dashing to avoid falling
            _previousGravityScale = _rigidbody.gravityScale;
            _rigidbody.gravityScale = 0f;

            // Updates states
            _isDashing = true;
            _isDashAvailable = false;
            _hasDashedInAir = !_isGrounded;

            animator.SetTrigger("Dashing");

            tr.startColor = Color.red; //DEBUG

            StartCoroutine(Dashing());
        }
    }

    // DEBUG
    private void OnNoclip(InputAction.CallbackContext ctx)
    {
        _noclip = !_noclip;

        if (_noclip)
        {
            _rigidbody.gravityScale = 0f;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            _rigidbody.gravityScale = 3f;
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public void OnHook()
    {
        _canMove = false;
    }

    public void OnUnHook()
    {
        _canMove = true;
    }

    public void OnEndHookMovement()
    {
        _isGrounded = true; 
        animator.SetBool("Is Grounded", _isGrounded);
    }

    private void Update()
    {
        Vector2 vel = Vector2.zero; //useless but necessary for the SmoothDamp
        _smoothMovement = Vector2.SmoothDamp(_smoothMovement, _movementInput, ref vel, _smoothInput);

        // Stops the jump if held for too long
        if (_isJumping && _jumpClock + maxJumpHold < Time.time)
            _isJumping = false;

        // Turns the character model around when facing the other direction
        Quaternion from = modelTransform.localRotation;
        Quaternion toIdle = _isFacingRight ? Quaternion.Euler(new Vector3(0f, 89.9f, 0f)) : Quaternion.Euler(new Vector3(0f, _backOrientationIdle, 0f));
        Quaternion toRunning = _isFacingRight ? Quaternion.Euler(new Vector3(0f, 89.9f, 0f)) : Quaternion.Euler(new Vector3(0f, _backOrientationRunning, 0f));
        // Lerps between a given orientation when idle facing left and when running facing left
        // This is used because facing left would normally put the back of the model towards the camera -> not fancy !!
        Quaternion to = _movementInput.x == 0f && !_isDashing ?
            Quaternion.Lerp(toIdle, toRunning, 0f)
            : Quaternion.Lerp(toRunning, toIdle, 0f);
        modelTransform.localRotation = Quaternion.Lerp(from, to, Time.deltaTime * _turnSpeed);

        UpdateStates();
    }

    private void FixedUpdate()
    {
        // Moves the player horizontally with according speeds while not dashing
        if (!_isDashing && _canMove)
        {
            if (_isGrounded)
                _rigidbody.velocity = new Vector2(_smoothMovement.x * horizontalSpeed, _rigidbody.velocity.y);
            else
                _rigidbody.velocity = new Vector2(_smoothMovement.x * inAirHorizontalSpeed, _rigidbody.velocity.y);
        }

        // DEBUG
        if (_noclip)
            _rigidbody.velocity = _movementInput * noclipSpeed;

        // Moves the player upward while holding the jump button
        if (_isJumping)
            _rigidbody.AddForce(Vector2.up * heldJumpForce, ForceMode2D.Force);
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
        if (_smoothMovement.x != 0f)
        {
            _isFacingRight = Mathf.Sign(_smoothMovement.x) == 1f;
        }

        // Updates whether the player is running or not
        if (_smoothMovement.x != 0f && Mathf.Abs(_rigidbody.velocity.x) > 0.01f) _isRunning = true;
        else _isRunning = false;

        if (!_canMove) return;
        
        // Updates the grounded state - check if one or both "feet" are on a ground
        _isGrounded = Physics2D.OverlapCircle(rightGroundCheck.position, groundCheckRadius, groundLayer)
                      || Physics2D.OverlapCircle(leftGroundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("Is Grounded", _isGrounded);
    }

    // Coroutine for the duration of the dash (not much use for now)
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

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

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
        Gizmos.DrawWireSphere(rightGroundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(leftGroundCheck.position, groundCheckRadius);

        // Displays the velocity
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + _rigidbody.velocity);

        // Displays stats on top of the player
        Handles.Label(_rigidbody.position + Vector2.up * 3.6f, "IsJumping : " + _isJumping, _isJumping ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 3.4f, "IsDashing : " + _isDashing, _isDashing ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 3.2f, "HasDashedInAir : " + _hasDashedInAir, _hasDashedInAir ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 3f, "IsDashAvailable : " + _isDashAvailable, _isDashAvailable ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.8f, "IsRunning : " + _isRunning, _isRunning ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.6f, "IsFacingRight : " + _isFacingRight, _isFacingRight ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.4f, "IsGrounded : " + _isGrounded, _isGrounded ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.2f, "Speed : " + _rigidbody?.velocity.magnitude + " m/s", _rigidbody.velocity.magnitude != 0f ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2f, "NOCLIP (O) : " + _noclip, _noclip ? greenStyle : redStyle);
    }

}
