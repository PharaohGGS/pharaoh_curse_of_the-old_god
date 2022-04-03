using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;

[CreateAssetMenu(fileName = "InputReader", menuName = "InputReader")]
public class InputReader : ScriptableObject, PlayerInput.ICharacterControlsActions, PlayerInput.ICharacterActionsActions, PlayerInput.IGameActions
{

    private PlayerInput _playerInput;

    public UnityAction<Vector2> movePerformedEvent;
    public UnityAction<Vector2> moveCanceledEvent;

    public UnityAction jumpStartedEvent;
    public UnityAction jumpCanceledEvent;

    public UnityAction dashStartedEvent;

    public UnityAction noclipPerformedEvent;
    
    public UnityAction hookGrappleStartedEvent;
    public UnityAction hookGrapplePerformedEvent;

    public UnityAction hookInteractStartedEvent;
    public UnityAction hookInteractPerformedEvent;

    public UnityAction sandSoldierStartedEvent;
    public UnityAction sandSoldierPerformedEvent;
    public UnityAction sandSoldierCanceledEvent;
    public UnityAction killAllSoldiersStartedEvent;

    public UnityAction exitPerformedEvent;

    public InputAction hookGrapple { get; private set; }
    public InputAction hookInteract { get; private set; }
    public bool isFacingRight { get; private set; } = true;
    
    // Used to be OnEnable() method, but OnEnable() doesn't work in a build :/ Unity documentation wrong for years
    public void Initialize()
    {
        if (_playerInput == null)
        {
            _playerInput = new PlayerInput();
            _playerInput.CharacterControls.SetCallbacks(this);
            _playerInput.CharacterActions.SetCallbacks(this);
            _playerInput.Game.SetCallbacks(this);
        }
        _playerInput.CharacterControls.Enable();
        _playerInput.CharacterActions.Enable();
        _playerInput.Game.Enable();

        hookGrapple = _playerInput.CharacterActions.HookGrapple;
        hookInteract = _playerInput.CharacterActions.HookInteract;
    }

    public void OnHookGrapple(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) hookGrappleStartedEvent?.Invoke();
        else if (context.phase == InputActionPhase.Performed) hookGrapplePerformedEvent?.Invoke();
    }

    public void OnHookInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) hookInteractStartedEvent?.Invoke();
        else if (context.phase == InputActionPhase.Performed) hookInteractPerformedEvent?.Invoke();
    }

    public void OnSandSoldier(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                sandSoldierStartedEvent?.Invoke();
                break;
            case InputActionPhase.Performed:
                sandSoldierPerformedEvent?.Invoke();
                break;
            case InputActionPhase.Canceled:
                sandSoldierCanceledEvent?.Invoke();
                break;
            default:
                break;
        }
    }

    public void OnKillAllSoldiers(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) killAllSoldiersStartedEvent?.Invoke();
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

    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && exitPerformedEvent != null)
            exitPerformedEvent.Invoke();
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
