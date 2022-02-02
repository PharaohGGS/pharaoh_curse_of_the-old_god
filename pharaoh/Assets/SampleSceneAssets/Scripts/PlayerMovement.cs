using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))] //auto creates a Rigidbody2D component when attaching this component
public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D _rigidbody;
    private float _horizontalMovement;
    private bool _jump;
    private bool _facingRight = true;

    [Header("Key Bindings")]

    public InputAction horizontalInput;
    public InputAction jumpInput;

    [Header("Movement metrics")]

    [Tooltip("100 horizontal speed to get 2m/s")]
    public float horizontalSpeed = 100f;

    public float inAirHorizontalSpeed;

    [Tooltip("15.1 to get a 3m70 jump (feet position)")]
    public float jumpForce = 15.1f;

    [Header("Ground Detection")]

    public Transform groundCheck;
    [Tooltip("0.05 to get a fine ground detection, keep it small and precise")]
    public float groundCheckRadius = 0.05f;
    public LayerMask groundLayer;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        jumpInput.performed += Jump;
    }

    private void Update()
    {
        _horizontalMovement = horizontalInput.ReadValue<float>();

        if (_horizontalMovement != 0f)
        {
            _facingRight = Mathf.Sign(_horizontalMovement) == 1f;
        }
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            _rigidbody.velocity = new Vector2(_horizontalMovement * horizontalSpeed * Time.fixedDeltaTime, _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.velocity = new Vector2(_horizontalMovement * inAirHorizontalSpeed * Time.fixedDeltaTime, _rigidbody.velocity.y);
        }
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (IsGrounded())
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpForce);
        }
    }

    private bool IsGrounded()
    {

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (_rigidbody == null) return;

        GUIStyle redStyle = new GUIStyle();
        redStyle.normal.textColor = Color.red;

        GUIStyle greenStyle = new GUIStyle();
        greenStyle.normal.textColor = Color.green;

        //Ground check radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        //Velocity direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(transform.position, _rigidbody.velocity.normalized));

        //Stats
        Handles.Label(transform.position + Vector3.up * 2f, "IsGrounded : " + IsGrounded(), IsGrounded() ? greenStyle : redStyle);
        Handles.Label(transform.position + Vector3.up * 2.5f, "Speed : " + _rigidbody?.velocity.magnitude + " m/s", _rigidbody.velocity.magnitude != 0f ? greenStyle : redStyle);
    }

    private void OnEnable()
    {
        horizontalInput.Enable();
        jumpInput.Enable();
    }

    private void OnDisable()
    {
        horizontalInput.Disable();
        jumpInput.Disable();
    }

}
