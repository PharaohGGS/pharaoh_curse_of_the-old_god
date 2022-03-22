using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirtualCameraHandler : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private Vector3 _cameraOffset;

    private void Start()
    {
        if (!TryGetComponent(out _virtualCamera))
            Debug.Log("No CinemachineVirtualCamera component found.");
        
        _virtualCamera.Follow = CameraManager.Instance.vcamFollowOffset.transform;
    }

    private void Update()
    {
        bool isFacingRight = CameraManager.Instance.player.GetComponent<PlayerMovement>().isFacingRight;
        CameraManager.Instance.vcamFollowOffset.transform.position = CameraManager.Instance.player.transform.position + CameraManager.Instance.cameraOffset * (isFacingRight ? 1 : -1);
    }
}
