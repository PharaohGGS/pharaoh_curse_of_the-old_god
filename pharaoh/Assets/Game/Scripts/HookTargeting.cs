using System;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using MessageType = Pharaoh.Tools.Debug.MessageType;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerMovement))]
public class HookTargeting : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField, Tooltip("Distance to hooked transform")] 
    private float offsetHook = 0.5f;
    [SerializeField] private float speed = 19f;
    private Transform _hooked;
    private bool _isOnHook = false;

    private float _gravityScale;
    private Coroutine _moveToHook;
    private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

    private PlayerInput _playerInput;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private PlayerMovement _playerMovement;
    private GameObject _bestTargetRight;
    private GameObject _bestTargetLeft;

    [Header("Detection")]

    [Tooltip("Target layers")]
    public LayerMask whatIsTarget;
    [Tooltip("Wall layers")]
    public LayerMask whatIsWall;
    [Tooltip("Targeting radius")]
    public float targetingRadius = 6f;
    
    [Header("Events")]
    public UnityEvent onHook = new UnityEvent();
    public UnityEvent onUnHook = new UnityEvent();
    public UnityEvent onEndHookMovement = new UnityEvent();

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _playerMovement = GetComponent<PlayerMovement>();
    }
    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.CharacterActions.Hook.performed += OnHook;
        _playerInput.CharacterControls.Move.performed += OnMove;
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Dash.started += OnDash;
    }

    private void OnDisable()
    {
        _playerInput.CharacterActions.Hook.performed -= OnHook;
        _playerInput.CharacterControls.Move.performed -= OnMove;
        _playerInput.CharacterControls.Jump.started -= OnJump;
        _playerInput.CharacterControls.Dash.started -= OnDash;
        _playerInput.Disable();
    }

    private void Update()
    {
        if (!_isOnHook && _hooked) return; 
        SearchHookTargets();
    }

    private void OnDrawGizmos()
    {
        if (_rigidbody == null)
            return;

        // Draws the best target to the right (red if not the faced direction)
        Gizmos.color = _playerMovement.IsFacingRight ? new Color(1f, 0.7531517f, 0f, 1f) : new Color(1f, 0.7531517f, 0f, 0.1f);
        if (_bestTargetRight != null)
            Gizmos.DrawLine(_rigidbody.position, _bestTargetRight.transform.position);

        // Draws the best target to the left (red if not the faced direction)
        Gizmos.color = !_playerMovement.IsFacingRight ? new Color(1f, 0.7531517f, 0f, 1f) : new Color(1f, 0.7531517f, 0f, 0.1f);
        if (_bestTargetLeft != null)
            Gizmos.DrawLine(_rigidbody.position, _bestTargetLeft.transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (_rigidbody == null)
            return;

        // Draws a disc around the player displaying the targeting range
        Handles.color = new Color(1f, 0.7531517f, 0f, 1f);
        Handles.DrawWireDisc(_rigidbody.position, Vector3.forward, targetingRadius);
    }

    private void OnMove(InputAction.CallbackContext obj)
    {
        var axis = _playerInput.CharacterControls.Move.ReadValue<Vector2>();

        if (axis.y >= 0f || !_hooked) return;

        UnHook();
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        if (!_hooked) return;
        UnHook();
    }
    private void OnDash(InputAction.CallbackContext obj)
    {
        if (!_hooked) return;
        UnHook();
    }

    private void OnHook(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
    {
        // unhook the current hooked object if there is one
        if (_hooked) UnHook();
        // hook the nearest hookable objects if there is one
        if (_bestTargetLeft || _bestTargetRight) Hook();
    }

    private void Hook()
    {
        _hooked = null;
        if (_bestTargetLeft && !_playerMovement.IsFacingRight) _hooked = _bestTargetLeft.transform;
        if (_bestTargetRight && _playerMovement.IsFacingRight) _hooked = _bestTargetRight.transform;
        if (_bestTargetLeft && _bestTargetRight)
        {
            _hooked = _playerMovement.IsFacingRight 
                ? _bestTargetRight.transform : _bestTargetLeft.transform;
        }

        if (!_hooked) return;
        
        _gravityScale = _rigidbody.gravityScale;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = 0;
        
        LogHandler.SendMessage($"hooking to {_hooked.name}", MessageType.Log);

        _moveToHook = StartCoroutine(MoveToHook(_hooked));
        onHook?.Invoke();
    }

    private void UnHook()
    {
        LogHandler.SendMessage($"unhooking from {_hooked.name}", MessageType.Log);
        if (_moveToHook != null) StopCoroutine(_moveToHook);

        _hooked = null;
        _isOnHook = false;
        _rigidbody.gravityScale = _gravityScale;
        onUnHook?.Invoke();
    }

    private void SearchHookTargets()
    {
        // Look for targets in targeting range
        Collider2D[] hits = Physics2D.OverlapCircleAll(_rigidbody.position, targetingRadius, whatIsTarget);

        // Loops each target and remove those behind walls as well as selects the closest one
        // Selects the best target to the right of the player and to the left
        int bestIdxRight = -1, bestIdxLeft = -1;
        float closestDistanceRight = float.MaxValue, closestDistanceLeft = float.MaxValue;
        for (int i = 0; i < hits.Length; i++)
        {
            if (_hooked == hits[i].transform) continue;

            bool isToTheRight = hits[i].transform.position.x > _rigidbody.position.x; //is the target to the right of the player ?
            Vector2 direction = ((Vector2)hits[i].transform.position - _rigidbody.position).normalized; //direction player -> target
            float distance = Vector2.Distance(_rigidbody.position, (Vector2)hits[i].transform.position); //distance player -> target

            if (isToTheRight
                && distance < closestDistanceRight
                && Physics2D.RaycastAll(_rigidbody.position, direction, distance, whatIsWall).Length < 1)
            {
                bestIdxRight = i;
                closestDistanceRight = distance;
            }
            else if (!isToTheRight
                     && distance < closestDistanceLeft
                     && Physics2D.RaycastAll(_rigidbody.position, direction, distance, whatIsWall).Length < 1)
            {
                bestIdxLeft = i;
                closestDistanceLeft = distance;
            }
        }

        // Selects the best targets if there is
        _bestTargetRight = bestIdxRight == -1 ? null : hits[bestIdxRight].gameObject;
        _bestTargetLeft = bestIdxLeft == -1 ? null : hits[bestIdxLeft].gameObject;
    }

    private System.Collections.IEnumerator MoveToHook(Transform hooked)
    {
        if (!hooked || !_rigidbody) yield break;

        while (Vector3.Distance(transform.position, hooked.position) > offsetHook)
        {
            _isOnHook = false;
            var position = Vector2.zero;
            var direction = hooked.position - transform.position;
            var distance = Vector3.Distance(transform.position, hooked.position);
            var velocity = new Vector2(direction.x, direction.y);
            var hit2Ds = Physics2D.RaycastAll(_rigidbody.position, direction, distance, whatIsWall);
            
            if (hit2Ds.Length > 0)
            {
                if (hit2Ds.Length > 1)
                {
                    Array.Sort(hit2Ds, (x, y) => x.distance.CompareTo(y.distance));
                }

                var bounds = hit2Ds[0].collider.bounds;
                var bottom = bounds.center - Vector3.up * bounds.extents.y;
                var top = bounds.center + Vector3.up * bounds.extents.y;

                var distTop = Vector3.Distance(transform.position, top);
                var distBottom = Vector3.Distance(transform.position, bottom);
                position.y += distTop < distBottom ? distTop : -distBottom;
            }

            _rigidbody.MovePosition(_rigidbody.position + position + velocity.normalized * (speed * Time.fixedDeltaTime));
            yield return _waitForFixedUpdate;
        }

        _isOnHook = true;
        onEndHookMovement?.Invoke();
    }

}
