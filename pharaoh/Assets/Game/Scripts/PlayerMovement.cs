using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))] //auto creates a Rigidbody2D component when attaching this component
public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D _rigidbody;
    private Vector2 _movement;
    private Vector2 _smoothMovement;
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

    [Tooltip("100 horizontal speed to get 2m/s")]
    public float horizontalSpeed = 100f;

    public float inAirHorizontalSpeed;

    [Tooltip("15.1 to get a 3m70 jump (feet position)")]
    public float jumpForce = 15.1f;

    [Tooltip("Smooths the player movement, 0.03 works well")]
    public float smoothInput = 0.03f;

    [Tooltip("Stops the player movement when in this range and no horizontal input is held")]
    public float movementDeadRange = 0.5f;

    [Header("Ground Detection")]

    public Transform groundCheck;

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

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _movement = movementInput.ReadValue<Vector2>();
        var vel = Vector2.zero; //unused but necessary
        _smoothMovement = Vector2.SmoothDamp(_smoothMovement, _movement, ref vel, smoothInput); //smooths the input to get a movement acceleration

        if (jumpInput.triggered && IsGrounded() && !_isDashing)
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);

        if (dashInput.triggered && !_isDashing && _isDashAvailable && !_hasDashedInAir)
        {
            _rigidbody.velocity = new Vector2(dashForce * Mathf.Sign(_smoothMovement.x), _rigidbody.velocity.y);
            _isDashing = true;
            _isDashAvailable = false;

            if (!IsGrounded())
                _hasDashedInAir = true;

            StartCoroutine(Dashing());
        }

        if (_hasDashedInAir && IsGrounded())
            _hasDashedInAir = false;

        // Cuts off the smoothdamp movement when decelerating
        if (_movement.x == 0f && _smoothMovement.x > -movementDeadRange && _smoothMovement.x < movementDeadRange)
            _smoothMovement.x = 0f;

        // Updates states
        if (_movement.x != 0f)
        {
            _isFacingRight = Mathf.Sign(_smoothMovement.x) == 1f;
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
        }
    }

    private void FixedUpdate()
    {
        // Moves the player horizontally with different speeds whil in-air
        if (!_isDashing)
        {
            if (IsGrounded())
                _rigidbody.velocity = new Vector2(_smoothMovement.x * horizontalSpeed, _rigidbody.velocity.y);
            else
                _rigidbody.velocity = new Vector2(_smoothMovement.x * inAirHorizontalSpeed, _rigidbody.velocity.y);
        }
    }

    //Returns whether the player is currently on a ground or not
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    System.Collections.IEnumerator Dashing()
    {
        yield return new WaitForSeconds(dashTime);

        _isDashing = false;

        StartCoroutine(DashCooldown());
    }

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

    private void OnDrawGizmosSelected()
    {
        if (_rigidbody == null) return;

        GUIStyle redStyle = new GUIStyle();
        redStyle.normal.textColor = Color.red;

        GUIStyle greenStyle = new GUIStyle();
        greenStyle.normal.textColor = Color.green;

        //Ground check radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        //Velocity direction
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + _rigidbody.velocity);

        //Stats
        Handles.Label(_rigidbody.position + Vector2.up * 3.2f, "IsDashing : " + _isDashing, _isDashing ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 3f, "HasDashedInAir : " + _hasDashedInAir, _hasDashedInAir ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.8f, "IsDashAvailable : " + _isDashAvailable, _isDashAvailable ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.6f, "IsRunning : " + _isRunning, _isRunning ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.4f, "IsFacingRight : " + _isFacingRight, _isFacingRight ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2.2f, "IsGrounded : " + IsGrounded(), IsGrounded() ? greenStyle : redStyle);
        Handles.Label(_rigidbody.position + Vector2.up * 2f, "Speed : " + _rigidbody?.velocity.magnitude + " m/s", _rigidbody.velocity.magnitude != 0f ? greenStyle : redStyle);
    }

}
