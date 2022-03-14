using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[RequireComponent(typeof(PlayerMovement))]
public class MovingBlockTargeting : Pharaoh.Gameplay.Targeting
{
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerMovement _playerMovement;
    private GameObject _movingBlock = null;

    [Header("Pulling")]

    public float pullForce = 1f;
    public float pullDuration = 1f;
    private WaitForSeconds _waitPullDuration;

    public GameObject hookIndicator;

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _rigidbody);
        _waitPullDuration = new WaitForSeconds(pullDuration);
        _playerInput = new PlayerInput();
        _playerInput.CharacterActions.HookBlock.started += HookToBlock;
        _playerInput.CharacterActions.HookBlock.performed += Pull;
        _playerMovement = GetComponent<PlayerMovement>();

        hookIndicator = Instantiate(hookIndicator);
    }

    private void Update()
    {
        SearchTargets();

        if (_playerMovement.IsHookedToBlock)
        {
            if (_playerMovement.IsRunning || _playerMovement.IsJumping || _playerMovement.IsDashing)
            {
                UnHook();
            }
        }

        if (_playerMovement.IsFacingRight && _bestTargetRight != null) //facing right with right target
            hookIndicator.transform.position = _bestTargetRight.transform.position;
        else if (_playerMovement.IsFacingRight && _bestTargetRight == null) //facing right without right target
            hookIndicator.transform.position = _bestTargetLeft != null ? _bestTargetLeft.transform.position : new Vector3(-1000f, -1000f, 0f);
        else if (!_playerMovement.IsFacingRight && _bestTargetLeft != null) //facing left with left target
            hookIndicator.transform.position = _bestTargetLeft.transform.position;
        else if (!_playerMovement.IsFacingRight && _bestTargetLeft == null) //facing left without left target
            hookIndicator.transform.position = _bestTargetRight != null ? _bestTargetRight.transform.position : new Vector3(-1000f, -1000f, 0f);
        else //none of the above
            hookIndicator.transform.position = new Vector3(-1000f, -1000f, 0f);
    }

    private void HookToBlock(InputAction.CallbackContext ctx)
    {
        if (_playerMovement.IsFacingRight)
            _movingBlock = _bestTargetRight != null ? _bestTargetRight : _bestTargetLeft;
        else
            _movingBlock = _bestTargetLeft != null ? _bestTargetLeft : _bestTargetRight;

        if (_playerMovement.isGrounded && _movingBlock != null)
        {
            _playerMovement.IsHookedToBlock = true;
            _playerMovement.IsFacingRight = _movingBlock.transform.position.x > transform.position.x;
        }
    }

    private void Pull(InputAction.CallbackContext ctx)
    {
        if (_playerMovement.IsHookedToBlock && !_playerMovement.IsPullingBlock && CanPullBlock())
            StartCoroutine(PullingBlock());
    }

    private System.Collections.IEnumerator PullingBlock()
    {
        if (!_movingBlock.transform.parent.TryGetComponent(out Rigidbody2D block) || 
            !_movingBlock.transform.parent.TryGetComponent(out MovingBlock mb))
        {
            yield break;
        }

        while (_playerInput.CharacterActions.HookBlock.IsPressed())
        {
            block.gravityScale = 0f;
            block.velocity = (_playerMovement.IsFacingRight ? Vector2.left : Vector2.right) * pullForce;
            block.constraints = RigidbodyConstraints2D.FreezeRotation;
            _playerMovement.IsPullingBlock = true;

            yield return _waitPullDuration;

            // test if something is between the block & rigidbody
            Vector2 direction = _rigidbody.position - block.position;
            if (Physics2D.RaycastAll(_rigidbody.position, direction.normalized, direction.magnitude, whatIsObstacle).Length > 0) break;
            if (!_movingBlock || !CanPullBlock() || !mb.IsGrounded()) break;
        }

        // Cancels pulling if not holding the button
        block.gravityScale = 1f;
        block.velocity = Vector2.zero;
        block.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        _playerMovement.IsPullingBlock = false;
    }

    private void UnHook()
    {
        _movingBlock = null;
        _playerMovement.IsHookedToBlock = false;
    }

    private bool CanPullBlock()
    {
        return _movingBlock != null && (_movingBlock.transform.position.x > (transform.position.x + 1.5f) || _movingBlock.transform.position.x < (transform.position.x - 1.5f));
    }

    private void OnDrawGizmos()
    {
        if (!_playerMovement) return;

        if (!_playerMovement.IsHookedToBlock)
        {
            // Draws the best target to the right (red if not the faced direction)
            Gizmos.color = _playerMovement.isFacingRight
                ? new Color(0f, 0.7531517f, 0f, 1f)
                : new Color(0f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetRight != null)
                Gizmos.DrawLine(transform.position, _bestTargetRight.transform.position);

            // Draws the best target to the left (red if not the faced direction)
            Gizmos.color = !_playerMovement.isFacingRight
                ? new Color(0f, 0.7531517f, 0f, 1f)
                : new Color(0f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetLeft != null)
                Gizmos.DrawLine(transform.position, _bestTargetLeft.transform.position);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _movingBlock.transform.position);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = new Color(0f, 0.5f, 0f);
            style.alignment = TextAnchor.MiddleCenter;
            Handles.Label(_movingBlock.transform.position + Vector3.up * 3f, "| Pulling bounds |", style);

            Debug.DrawLine(_movingBlock.transform.position + new Vector3(1.5f, -3f, 0f), _movingBlock.transform.position + new Vector3(1.5f, 3f, 0f), new Color(0f, 0.5f, 0f));
            Debug.DrawLine(_movingBlock.transform.position + new Vector3(-1.5f, -3f, 0f), _movingBlock.transform.position + new Vector3(-1.5f, 3f, 0f), new Color(0f, 0.5f, 0f));
        }
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

}
