using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirtualCameraHandler : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private void Start()
    {
        if (!TryGetComponent(out CinemachineVirtualCamera virtualCamera))
            Debug.Log("No CinemachineVirtualCamera component found.");
        else
            _virtualCamera = virtualCamera;
        _virtualCamera.Follow = CameraManager.Instance.playerTransform;
    }
}
