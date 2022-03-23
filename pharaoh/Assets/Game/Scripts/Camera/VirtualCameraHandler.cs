using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Managers;
using UnityEngine;

public class VirtualCameraHandler : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private Vector3 _cameraOffset;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = CameraManager.Instance.player.GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        if (!TryGetComponent(out _virtualCamera))
            Debug.Log("No CinemachineVirtualCamera component found.");
        
        _virtualCamera.Follow = CameraManager.Instance.vcamFollowOffset.transform;
    }

    private void Update()
    {
        CameraManager.Instance.vcamFollowOffset.transform.position = CameraManager.Instance.player.transform.position + CameraManager.Instance.cameraOffset * (_playerMovement.IsFacingRight ? 1 : -1);
    }
}
