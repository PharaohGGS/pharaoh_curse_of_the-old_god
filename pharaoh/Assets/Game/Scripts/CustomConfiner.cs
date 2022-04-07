using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CustomConfiner : MonoBehaviour
{
    public Camera mainCamera;
    public Transform player;

    private BoxCollider2D _cameraBox;
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineTransposer _transposer;
    private Vector3 offset;

    [Header("Camera options")]
    [Range(1.0f, 100.0f)] public float frustumHeight;
    [Range(1.0f, 100.0f)] public float distance;

    private void Start()
    {
        if (mainCamera.TryGetComponent(out BoxCollider2D boxCollider2D))
            _cameraBox = boxCollider2D;
        if (TryGetComponent(out CinemachineVirtualCamera virtualCamera))
        {
            _virtualCamera = virtualCamera;
            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        }
        
        SetupCamera();
    }

    private void Update()
    {
        SetupCamera();
        //Confine();
    }

    private void SetupCamera()
    {
        _virtualCamera.m_Lens.FieldOfView = 2.0f * Mathf.Atan(frustumHeight * 0.5f / distance) * Mathf.Rad2Deg;
        
        offset = new Vector3 { z = -1.0f * distance };
        _transposer.m_FollowOffset = offset;
    }

    private void Confine()
    {
        // Bounds roomBounds = CameraManager.Instance.CurrentRoomBoundaries;
        // float frustumWidth = frustumHeight * mainCamera.aspect;
        //
        // Transform follow = _virtualCamera.Follow;
        //
        // if ()
        //
        // _transposer.m_FollowOffset = new Vector3(offset.x, offset.y, -3.5f);
    }
    
    private void OnDrawGizmos()
    {
        Vector3 center = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector3 size = new Vector3(frustumHeight * mainCamera.aspect, frustumHeight, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}
