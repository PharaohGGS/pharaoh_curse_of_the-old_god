using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Pharaoh.Gameplay.Components.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;

public class LookUpDown : MonoBehaviour
{

    public InputReader inputReader;
    public GameObject virtualCamera;

    public float upValue = 2f;
    public float downValue = 2f;
    
    private void OnEnable()
    {
        inputReader.lookPerformedEvent += OnLook;
    }

    private void OnDisable()
    {
        inputReader.lookPerformedEvent -= OnLook;
    }
    
    private void OnLook(float value)
    {
        Vector3 position = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.State.CorrectedPosition;
        if (Vector3.Distance(position, Camera.main.transform.position) > 0.1f)
        {
            EnableActions();
            virtualCamera.SetActive(false);
            return;
        }
            
        switch (value)
        {
            case > 0f:
                DisableActions();
                position.y += upValue;
                virtualCamera.transform.position = position;
                virtualCamera.SetActive(true);
                break;
            case < 0f:
                TryGetComponent(out PlayerMovement playerMovement);
                if (playerMovement.IsHooking) break;
                DisableActions();
                position.y -= downValue;
                virtualCamera.transform.position = position;
                virtualCamera.SetActive(true);
                break;
            default:
                EnableActions();
                virtualCamera.SetActive(false);
                break;
        }
    }

    private void DisableActions()
    {
        inputReader.DisableMove();
        inputReader.DisableDash();
        inputReader.DisableJump();
        inputReader.DisableHookGrapple();
        inputReader.DisableHookInteract();
        inputReader.DisableSandSoldier();
    }
    
    private void EnableActions()
    {
        inputReader.EnableMove();
        inputReader.EnableDash();
        inputReader.EnableJump();
        inputReader.EnableHookGrapple();
        inputReader.EnableHookInteract();
        inputReader.EnableSandSoldier();
    }
}
