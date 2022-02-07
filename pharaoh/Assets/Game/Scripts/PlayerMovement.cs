using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))] //auto creates a Rigidbody2D component when attaching this component
public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D _rigidbody;
    private Vector2 _smoothMovement;
    private bool _isGrounded = false;
    private bool _isRunning = false;
    private bool _isFacingRight = true;
    private bool _isDashing = false;
    private bool _hasDashedInAir = false;
    private bool _isDashAvailable = true;

    [Header("Key Bindings")]

    public InputAction movementInput;
    public InputAction jumpInput;
    public InputAction dashInput;

    [Header("Movement")]

    [Tooltip("5m/s : given metrics")]
    public float horizontalSpeed = 100f;

    [Tooltip("5m/s : given metrics")]
    public float inAirHorizontalSpeed = 5f;

    [Tooltip("15.1 to get a 3m70 jump (feet position)")]
    public float jumpForce = 15.1f;

    [Tooltip("Smooths the player movement, 0.03 works well")]
    public float smoothInput = 0.03f;

    [Tooltip("Stops the player movement when in this range and no horizontal input is held")]
    public float movementDeadRange = 0.5f;

    [Header("Ground Detection")]

    [Tooltip("Rightmost ground check")]
    public Transform rightGroundCheck;

    [Tooltip("Leftmost ground check")]
    public Transform leftGroundCheck;

    [Tooltip("0.05 to get a fine ground detection, keep it small and precise")]
    public float groundCheckRadius = 0.05f;

    public LayerMask groundLayer;

    [Header("Dash")]

    [Tooltip("Dashing force, 50 works well")]
    public float dashForce = 5f;

    [Tooltip("Dashing time, 0.1 works well")]
    public float dashTime = 0.1f;

    [Tooltip("Cooldown between each dash, starts at the end of the previous one")]
    public float dashCooldown = 0.5f;

    [Header("Animations")]

    [Tooltip("Animator controlling the player")]
    public Animator animator;

    [Tooltip("Model transform to turn the player around")]
    public Transform modelTransform;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Recover the player's input and smooths it - vel is unused but necessary
        Vector2 movement = movementInput.ReadValue<Vector2>().normalized, vel = Vector2.zero;
        _smoothMovement = Vector2.SmoothDamp(_smoothMovement, movement, ref vel, smoothInput);

        // Jumping
        if (jumpInput.triggered && _isGrounded && !_isDashing)
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Dashing
        if (dashInput.triggered && !_isDashing && _isDashAvailable && !_hasDashedInAir)
        {
            // Resets the velocity and adds the dash force towards facing direction
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.AddForce((_isFacingRight ? Vector2.right : Vector2.left) * dashForce, ForceMode2D.Impulse);

            _isDashing = true;
            _isDashAvailable = false;
            _hasDashedInAir = !_isGrounded;

            StartCoroutine(Dashing());
        }

        // Cuts off the smoothdamp movement when decelerating
        if (movement.x == 0f && _smoothMovement.x > -movementDeadRange && _smoothMovement.x < movementDeadRange)
            _smoothMovement.x = 0f;

        UpdateStates();
    }

    private void FixedUpdate()
    {
        // Moves the player horizontally with according speeds while not dashing
        if (!_isDashing)
        {
            if (_isGrounded)
                _rigidbody.velocity = new Vector2(_smoothMovement.x * horizontalSpeed, _rigidbody.velocity.y);
            else
                _rigidbody.velocity = new Vector2(_smoothMovement.x * inAirHorizontalSpeed, _rigidbody.velocity.y);
        }
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
            modelTransform.localScale = _isFacingRight? new Vector3(1f, 1f, 1f) : new Vector3(1f, 1f, -1f);
        }

        // Updates whether the player is running or not
        if (_smoothMovement.x != 0f) _isRunning = true;
        else _isRunning = false;
        animator.SetBool("Is Running", _isRunning);


        // Updates the grounded state - check if one or both "feet" are on a ground
        _isGrounded = Physics2D.OverlapCircle(rightGroundCheck.position, groundCheckRadius, groundLayer)
           || Physics2D.OverlapCircle(leftGroundCheck.position, groundCheckRadius, groundLayer);
    }

    // Coroutine for the duration of the dash (not much use for now)
    System.Collections.IEnumerator Dashing()
    {
        yield return new WaitForSeconds(dashTime);

        _isDashing = false;

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
        movementInput.Enable();
        jumpInput.Enable();
        dashInput.Enable();
    }

    private void OnDisable()
    {
        movementInput.Disable();
        jumpInput.Disable();
        dashInput.Disable();
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
        Handles.Label(_rigidbody.position + Vector2.up * 3.2f, "IsDashing : " + _isDashing, _isDashing ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 3f, "HasDashedInAir : " + _hasDashedInAir, _hasDashedInAir ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.8f, "IsDashAvailable : " + _isDashAvailable, _isDashAvailable ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.6f, "IsRunning : " + _isRunning, _isRunning ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.4f, "IsFacingRight : " + _isFacingRight, _isFacingRight ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.2f, "IsGrounded : " + _isGrounded, _isGrounded ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2f, "Speed : " + _rigidbody?.velocity.magnitude + " m/s", _rigidbody.velocity.magnitude != 0f ? greenStyle : redStyle);
    }

}
