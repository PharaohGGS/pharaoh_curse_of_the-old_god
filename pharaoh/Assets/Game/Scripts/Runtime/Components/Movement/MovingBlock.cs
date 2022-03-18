using System;
using System.Linq;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

public class MovingBlock : MonoBehaviour
{
    public LayerMask whatIsSpike;
    public LayerMask whatIsGround;

    [SerializeField] private float circleCastRadius = 0.3f;

    [SerializeField] private CircleCollider2D _rightHandle;
    [SerializeField] private CircleCollider2D _leftHandle;
    [SerializeField] private Transform _rightGroundCheck;
    [SerializeField] private Transform _leftGroundCheck;
    private float _groundCheckDistance = 0.05f;
    
    public UnityEvent onLeavingGround;
    public UnityEvent onTriggerGround;
    public UnityEvent onTriggerSpike;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;

    private bool _isGrounded;
    // Returns whether or not the block is grounded
    public bool isGrounded => _isGrounded;

    public bool isFalling => _rigidbody2D?.velocity.y < -0.1f && !isGrounded;

    private void Awake()
    {
        if (!TryGetComponent(out _collider2D))
        {
            Debug.LogError($"No collider on movingblock, this is not normal.");
        }

        if (!TryGetComponent(out _rigidbody2D))
        {
            Debug.LogError($"No rigidbody on movingblock, this is not normal.");
        }
    }

    public void FixedUpdate()
    {
        var leftHits = Physics2D.CircleCastAll(_leftGroundCheck.position, circleCastRadius, Vector2.down, _groundCheckDistance, whatIsGround);
        var rightHits = Physics2D.CircleCastAll(_rightGroundCheck.position, circleCastRadius, Vector2.down, _groundCheckDistance, whatIsGround);
        
        _isGrounded = leftHits.Length > leftHits.Count(leftHit => leftHit.transform == transform) || 
                      rightHits.Length > rightHits.Count(rightHit => rightHit.transform == transform);

        // if not equals change var and call event
        if (isFalling)
        {
            onLeavingGround?.Invoke();
            _rigidbody2D.velocity = Vector2.up * _rigidbody2D.velocity.y;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (whatIsSpike.HasLayer(other.gameObject.layer))
        {
            onTriggerSpike?.Invoke();
        }

        if (whatIsGround.HasLayer(other.gameObject.layer))
        {
            onTriggerGround?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (_rightGroundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_rightGroundCheck.position, circleCastRadius);
        }

        if (_leftGroundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_leftGroundCheck.position, circleCastRadius);
        }
    }

}
