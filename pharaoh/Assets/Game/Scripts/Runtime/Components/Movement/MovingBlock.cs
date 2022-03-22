using System;
using System.Linq;
using Pharaoh.Gameplay;
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

    public bool isPulled { get; private set; }
    public bool isHooked { get; private set; }
    public bool isGrounded { get; private set; }

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

    private void OnEnable()
    {
        // Hook bindings
        HookBehaviour.started += OnHookStarted;
        HookBehaviour.performed += OnHookPerformed;
        HookBehaviour.ended += OnHookEnded;
        HookBehaviour.released += OnHookReleased;
    }

    private void OnDisable()
    {
        // Hook bindings
        HookBehaviour.started -= OnHookStarted;
        HookBehaviour.performed -= OnHookPerformed;
        HookBehaviour.ended -= OnHookEnded;
        HookBehaviour.released -= OnHookReleased;
    }
    
    private void OnHookStarted(HookBehaviour behaviour)
    {
        if (!behaviour.isCurrentTarget || behaviour is not PullHookBehaviour pull) return;
        if (pull.gameObject != _rightHandle.gameObject && pull.gameObject != _leftHandle.gameObject) return;

        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        isHooked = true;
    }

    private void OnHookPerformed(HookBehaviour behaviour)
    {
        if (!behaviour.isCurrentTarget || behaviour is not PullHookBehaviour pull) return;
        if (pull.gameObject != _rightHandle.gameObject && pull.gameObject != _leftHandle.gameObject) return;
        
        isPulled = true;
        _rigidbody2D.MovePosition(pull.nextPosition);
    }

    private void OnHookEnded(HookBehaviour behaviour)
    {
        if (!behaviour.isCurrentTarget || behaviour is not PullHookBehaviour pull) return;
        if (pull.gameObject != _rightHandle.gameObject && pull.gameObject != _leftHandle.gameObject) return;

        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        _rigidbody2D.velocity = Vector2.up * _rigidbody2D.velocity.y;
        isPulled = false;
        isHooked = false;
    }
    
    private void OnHookReleased(HookBehaviour behaviour)
    {
        if (!behaviour.isCurrentTarget || behaviour is not PullHookBehaviour pull) return;
        if (pull.gameObject != _rightHandle.gameObject && pull.gameObject != _leftHandle.gameObject) return;

        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        isPulled = false;
        isHooked = false;
    }

    public void FixedUpdate()
    {
        var leftHits = Physics2D.CircleCastAll(_leftGroundCheck.position, circleCastRadius, Vector2.down, _groundCheckDistance, whatIsGround);
        var rightHits = Physics2D.CircleCastAll(_rightGroundCheck.position, circleCastRadius, Vector2.down, _groundCheckDistance, whatIsGround);
        
        isGrounded = leftHits.Length > leftHits.Count(leftHit => leftHit.transform == transform) || 
                      rightHits.Length > rightHits.Count(rightHit => rightHit.transform == transform);

        // if not equals change var and call event
        if (!isFalling) return;
        onLeavingGround?.Invoke();
        _rigidbody2D.velocity = Vector2.up * _rigidbody2D.velocity.y;
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
