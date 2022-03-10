using System;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

public class MovingBlock : MonoBehaviour
{
    public LayerMask whatIsSpike;
    public LayerMask whatIsGround;

    [SerializeField] private CircleCollider2D _rightHandle;
    [SerializeField] private CircleCollider2D _leftHandle;
    [SerializeField] private Transform _rightGroundCheck;
    [SerializeField] private Transform _leftGroundCheck;
    private float _groundCheckDistance = 0.05f;


    public UnityEvent onLeavingGround;
    public UnityEvent onTriggerGround;
    public UnityEvent onTriggerSpike;
    private bool _isGrounded = false;

    private void Awake()
    {
        //_rightHandle = transform.Find("DEBUG - Moving Block Target Right").GetComponent<CircleCollider2D>();
        //_leftHandle = transform.Find("DEBUG - Moving Block Target Left").GetComponent<CircleCollider2D>();
        //_rightGroundCheck = transform.Find("Right Ground Check").transform;
        //_leftGroundCheck = transform.Find("Left Ground Check").transform;
    }

    public void FixedUpdate()
    {
        // if not equals change var and call event
        var isGrounded = IsGrounded();
        if (_isGrounded == isGrounded) return;

        _isGrounded = isGrounded;
            
        if (!_isGrounded)
        {
            onLeavingGround?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (whatIsSpike.HasLayer(other.gameObject.layer))
        {
            //_rightHandle.enabled = false;
            //_leftHandle.enabled = false;
            onTriggerSpike?.Invoke();
        }

        if (whatIsGround.HasLayer(other.gameObject.layer))
        {
            onTriggerGround?.Invoke();
        }

        //this.enabled = false;
    }

    // Returns whether or not the block is grounded
    public bool IsGrounded()
    {
        return Physics2D.Raycast(_rightGroundCheck.position, Vector2.down, _groundCheckDistance, whatIsGround)
            || Physics2D.Raycast(_leftGroundCheck.position, Vector2.down, _groundCheckDistance, whatIsGround);
    }

}
