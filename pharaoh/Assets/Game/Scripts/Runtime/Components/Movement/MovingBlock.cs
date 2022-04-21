using System;
using System.Linq;
using Pharaoh.Gameplay;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;
using AudioManager = Pharaoh.Managers.AudioManager;

public class MovingBlock : MonoBehaviour
{

    public LayerMask whatIsSpike;
    public LayerMask whatIsGround;

    [SerializeField, Header("Hook Events")] 
    private HookBehaviourEvents hookEvents;

    [Header("Hook Handles")]
    [SerializeField] private CircleCollider2D _rightHandle;
    [SerializeField] private CircleCollider2D _leftHandle;
    [SerializeField] private float boxCastUpSize = 0.3f;
    [SerializeField] private float groundCheckDistance = 0.05f;
    
    [Header("Events")]
    public UnityEvent onLeavingGround;
    public UnityEvent onTriggerGround;
    public UnityEvent onTriggerSpike;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;

    private int _pullCountSave;

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

        if (!hookEvents)
        {
            Debug.LogWarning($"No hookEvents scriptableobject attach to this movingBlock, events will never be called.");
        }
    }

    private void OnEnable()
    {
        // Hook bindings
        if (!hookEvents) return;
        hookEvents.started += OnHookStarted;
        hookEvents.performed += OnHookPerformed;
        hookEvents.ended += OnHookEnded;
        hookEvents.released += OnHookReleased;
    }

    private void OnDisable()
    {
        // Hook bindings
        if (!hookEvents) return;
        hookEvents.started -= OnHookStarted;
        hookEvents.performed -= OnHookPerformed;
        hookEvents.ended -= OnHookEnded;
        hookEvents.released -= OnHookReleased;
    }
    
    private void OnHookStarted(HookBehaviour behaviour)
    {
        if (!behaviour.isCurrentTarget || behaviour is not PullHookBehaviour pull) return;
        if (behaviour.gameObject != _rightHandle.gameObject && behaviour.gameObject != _leftHandle.gameObject) return;

        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        isHooked = true;
    }

    private void OnHookPerformed(HookBehaviour behaviour)
    {
        if (!behaviour.isCurrentTarget || behaviour is not PullHookBehaviour pull) return;
        if (behaviour.gameObject != _rightHandle.gameObject && behaviour.gameObject != _leftHandle.gameObject) return;

        if (_pullCountSave != pull.pullCount)
        {
            _pullCountSave = pull.pullCount;
            AudioManager.Instance.Stop("CratePull");
        }

        AudioManager.Instance.Play("CratePull");
        
        isPulled = true;
        _rigidbody2D.MovePosition(behaviour.nextPosition);
    }

    private void OnHookEnded(HookBehaviour behaviour)
    {
        if (!behaviour.isCurrentTarget || behaviour is not PullHookBehaviour pull) return;
        if (behaviour.gameObject != _rightHandle.gameObject && behaviour.gameObject != _leftHandle.gameObject) return;

        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        _rigidbody2D.velocity = Vector2.up * _rigidbody2D.velocity.y;
        isPulled = false;
        isHooked = false;
    }
    
    private void OnHookReleased(HookBehaviour behaviour)
    {
        if (!behaviour.isCurrentTarget || behaviour is not PullHookBehaviour pull) return;
        if (behaviour.gameObject != _rightHandle.gameObject && behaviour.gameObject != _leftHandle.gameObject) return;

        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        isPulled = false;
        isHooked = false;
    }

    public void FixedUpdate()
    {
        var bounds = _collider2D.bounds;
        var center = (Vector2)transform.position - Vector2.up * bounds.extents.y;
        var sizeX = bounds.size.x * (1f + groundCheckDistance);
        var boxHits = Physics2D.BoxCastAll(center, new Vector2(sizeX, boxCastUpSize), 
            0, Vector2.down, groundCheckDistance, whatIsGround);
        isGrounded = boxHits.Length > boxHits.Count(boxHit => boxHit.transform == transform);

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
        if (TryGetComponent(out Collider2D coll))
        {
            Gizmos.color = Color.red;
            var bounds = coll.bounds;
            Gizmos.DrawWireCube(transform.position - Vector3.up * bounds.extents.y, 
                new Vector3(bounds.size.x * (1f + groundCheckDistance), boxCastUpSize, 0.0f));
        }
    }

}
