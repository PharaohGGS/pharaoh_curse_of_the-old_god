using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class MovingBlockTargeting : Pharaoh.Gameplay.Targeting
{

    private PlayerInput _playerInput;
    private PlayerMovement _playerMovement;
    private GameObject _movingBlock = null;

    [Header("Pulling")]

    public float pullForce = 1f;
    public float pullDuration = 1f;

    protected override void Awake()
    {
        base.Awake();
        _playerInput = new PlayerInput();
        _playerInput.CharacterActions.HookBlock.started += HookToBlock;
        _playerInput.CharacterActions.HookBlock.performed += Pull;
        _playerInput.CharacterControls.Move.started += UnHook;
        _playerInput.CharacterControls.Jump.started += UnHook;
        _playerInput.CharacterControls.Dash.started += UnHook;
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        SearchTargets();
    }

    private void HookToBlock(InputAction.CallbackContext ctx)
    {
        Debug.Log("Hooking to block ...");

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
        Debug.Log("Pulling block");

        if (!_playerMovement.IsPullingBlock)
            StartCoroutine(PullingBlock());
    }

    private System.Collections.IEnumerator PullingBlock()
    {
        Rigidbody2D block = _movingBlock.transform.parent.GetComponent<Rigidbody2D>();

        // Disables the block's gravity (avoiding friction) and assigning it a velocity to move
        block.gravityScale = 0f;
        block.velocity = (_playerMovement.IsFacingRight ? Vector2.left : Vector2.right) * pullForce;
        block.constraints = RigidbodyConstraints2D.FreezeRotation;

        _playerMovement.IsPullingBlock = true;

        yield return new WaitForSeconds(pullDuration);

        if (!_playerInput.CharacterActions.HookBlock.IsPressed())
        {
            // Cancels pulling if not holding the button
            block.gravityScale = 1f;
            block.velocity = Vector2.zero;
            block.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

            _playerMovement.IsPullingBlock = false;
        }
        else
        {
            // Continues pulling if holding the button
            StartCoroutine(PullingBlock());
        }
    }

    private void UnHook(InputAction.CallbackContext ctx)
    {
        _movingBlock = null;
        _playerMovement.IsHookedToBlock = false;
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
