using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class MovingBlockTargeting : Pharaoh.Gameplay.Targeting
{

    private PlayerInput _playerInput;
    private PlayerMovement _playerMovement;
    private GameObject _movingBlock = null;

    protected override void Awake()
    {
        base.Awake();
        _playerInput = new PlayerInput();
        _playerInput.CharacterActions.Grab.started += HookToBlock;
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
