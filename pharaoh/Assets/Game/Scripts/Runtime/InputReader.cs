using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;

[CreateAssetMenu(fileName = "InputReader", menuName = "InputReader")]
public class InputReader : ScriptableObject, PlayerInput.ICharacterControlsActions, PlayerInput.ICharacterActionsActions
{

    private PlayerInput _playerInput;

    public UnityAction<Vector2> movePerformedEvent;
    public UnityAction<Vector2> moveCanceledEvent;

    public UnityAction jumpStartedEvent;
    public UnityAction jumpCanceledEvent;

    public UnityAction dashStartedEvent;

    public UnityAction noclipPerformedEvent;
    
    public UnityAction hookGrapplePerformedEvent;
    public UnityAction hookInteractPerformedEvent;

    public UnityAction<float> lookPerformedEvent;
    public InputAction hookGrapple { get; private set; }
    public InputAction hookInteract { get; private set; }
    public bool isFacingRight { get; private set; } = true;
    
    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = new PlayerInput();
            _playerInput.CharacterControls.SetCallbacks(this);
            _playerInput.CharacterActions.SetCallbacks(this);
        }
        _playerInput.CharacterControls.Enable();
        _playerInput.CharacterActions.Enable();

        hookGrapple = _playerInput.CharacterActions.HookGrapple;
        hookInteract = _playerInput.CharacterActions.HookInteract;
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
        _playerInput.CharacterActions.Disable();
    }

    public void OnHookGrapple(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) hookGrapplePerformedEvent?.Invoke();
    }

    public void OnHookInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) hookInteractPerformedEvent?.Invoke();
    }

    public void OnSandSoldier(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();
        if (context.phase == InputActionPhase.Performed) lookPerformedEvent?.Invoke(value);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var axis = context.ReadValue<Vector2>();

        switch (context.phase)
        {
            case InputActionPhase.Performed:
                isFacingRight = axis.x >= 0;
                movePerformedEvent?.Invoke(axis);
                break;
            case InputActionPhase.Canceled:
                moveCanceledEvent?.Invoke(axis);
                break;
        }
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

    public void DisableHookGrapple()
    {
        _playerInput.CharacterActions.HookGrapple.Disable();
    }

    public void DisableHookInteract()
    {
        _playerInput.CharacterActions.HookInteract.Disable();
    }

    public void EnableHookGrapple()
    {
        _playerInput.CharacterActions.HookGrapple.Enable();
    }

    public void EnableHookInteract()
    {
        _playerInput.CharacterActions.HookInteract.Enable();
    }

    public void DisableSandSoldier()
    {
        _playerInput.CharacterActions.SandSoldier.Disable();
    }

    public void EnableSandSoldier()
    {
        _playerInput.CharacterActions.SandSoldier.Enable();
    }

    public override string ToString()
    {
        return "Controls :\n" +
            "\tMove : " + (_playerInput.CharacterControls.Move.enabled ? "enabled" : "disabled") + "\n" +
            "\tJump : " + (_playerInput.CharacterControls.Jump.enabled ? "enabled" : "disabled") + "\n" +
            "\tDash : " + (_playerInput.CharacterControls.Dash.enabled ? "enabled" : "disabled");
    }

}
