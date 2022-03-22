using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "InputReader")]
public class InputReader : ScriptableObject, PlayerInput.ICharacterControlsActions
{

    private PlayerInput _playerInput;

    public UnityAction<Vector2> movePerformedEvent;
    public UnityAction<Vector2> moveCanceledEvent;

    public UnityAction jumpStartedEvent;
    public UnityAction jumpCanceledEvent;

    public UnityAction dashStartedEvent;

    public UnityAction noclipPerformedEvent;

    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = new PlayerInput();
            _playerInput.CharacterControls.SetCallbacks(this);
        }
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && movePerformedEvent != null)
            movePerformedEvent.Invoke(context.ReadValue<Vector2>());

        if (context.phase == InputActionPhase.Canceled && moveCanceledEvent != null)
            moveCanceledEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && jumpStartedEvent != null)
            jumpStartedEvent.Invoke();

        if (context.phase == InputActionPhase.Canceled && jumpCanceledEvent != null)
            jumpCanceledEvent.Invoke();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && dashStartedEvent != null)
            dashStartedEvent.Invoke();
    }

    public void OnNOCLIP(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && noclipPerformedEvent != null)
            noclipPerformedEvent.Invoke();
    }

    public void DisableMove()
    {
        _playerInput.CharacterControls.Move.Disable();
    }

    public void EnableMove()
    {
        _playerInput.CharacterControls.Move.Enable();
    }

    public void DisableJump()
    {
        _playerInput.CharacterControls.Jump.Disable();
    }

    public void EnableJump()
    {
        _playerInput.CharacterControls.Jump.Enable();
    }

    public void DisableDash()
    {
        _playerInput.CharacterControls.Dash.Disable();
    }

    public void EnableDash()
    {
        _playerInput.CharacterControls.Dash.Enable();
    }

    public override string ToString()
    {
        return "Controls :\n" +
            "\tMove : " + (_playerInput.CharacterControls.Move.enabled ? "enabled" : "disabled") + "\n" +
            "\tJump : " + (_playerInput.CharacterControls.Jump.enabled ? "enabled" : "disabled") + "\n" +
            "\tDash : " + (_playerInput.CharacterControls.Dash.enabled ? "enabled" : "disabled");
    }

}
