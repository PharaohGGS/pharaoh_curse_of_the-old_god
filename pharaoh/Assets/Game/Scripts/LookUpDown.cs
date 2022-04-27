using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Pharaoh.Gameplay;
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
        inputReader.lookStartedEvent += OnLook;
        inputReader.lookCanceledEvent += OnStopLook;
    }

    private void OnDisable()
    {
        inputReader.lookStartedEvent -= OnLook;
        inputReader.lookCanceledEvent -= OnStopLook;
    }
    
    private void OnLook(float value)
    {
        Debug.Log("STARTED");
        Vector3 position = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.State.CorrectedPosition;
        if (Vector3.Distance(position, Camera.main.transform.position) > 0.1f)
        {
            EnableActions();
            virtualCamera.SetActive(false);
            return;
        }
        
        var brain = CinemachineCore.Instance.GetActiveBrain(0);
        var vCam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        var transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 followOffset = Vector3.zero;
        if (transposer != null)
            followOffset = vCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        float fov = vCam.m_Lens.FieldOfView;

        virtualCamera.TryGetComponent(out CinemachineVirtualCamera lookCam);
        lookCam.m_Lens.FieldOfView = fov;
        position.z = transform.position.z + followOffset.z;
        
        TryGetComponent(out PlayerMovement playerMovement);
        switch (value)
        {
            case > 0f:
                if (playerMovement.IsHooked || playerMovement.IsHooking) break;
                DisableActions();
                position.y += upValue;
                virtualCamera.transform.position = position;
                virtualCamera.SetActive(true);
                break;
            case < 0f:
                if (playerMovement.IsHooked || playerMovement.IsHooking) break;
                DisableActions();
                position.y -= downValue;
                virtualCamera.transform.position = position;
                virtualCamera.SetActive(true);
                break;
        }
    }

    private void OnStopLook()
    {
        Debug.Log("CANCELED");
        EnableActions();
        virtualCamera.SetActive(false);
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
