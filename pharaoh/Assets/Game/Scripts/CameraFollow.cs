using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    private Camera _camera;
    private BoxCollider2D _cameraBox;
    
    private Vector3 _velocity = Vector3.zero;
    private float _waitForSeconds = 0.5f;
    
    private void Start()
    {
        if (TryGetComponent(out Camera cameraComponent))
            _camera = cameraComponent;
        if (TryGetComponent(out BoxCollider2D boxCollider2D))
            _cameraBox = boxCollider2D;
    }

    private void Update()
    {
        CalculateCameraBox();
        if (_waitForSeconds > 0)
            _waitForSeconds -= Time.deltaTime;
        else
            FollowPlayer();
    }

    private void FollowPlayer()
    {
        Bounds targetBounds = CameraManager.Instance.CurrentRoomBoundaries;
        Vector2 size = _cameraBox.size;
        float xTarget = size.x < targetBounds.size.x ?
            Mathf.Clamp(player.position.x, targetBounds.min.x + size.x / 2, targetBounds.max.x - size.x / 2) :
            (targetBounds.min.x + targetBounds.max.x) / 2;
        float yTarget = size.y < targetBounds.size.y ?
            Mathf.Clamp(player.position.y, targetBounds.min.y + size.y / 2, targetBounds.max.y - size.y / 2) :
            (targetBounds.min.y + targetBounds.max.y) / 2;
        Vector3 target = new Vector3(xTarget, yTarget, transform.position.z);
        //transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, 0.2f);
    }

    private void CalculateCameraBox()
    {
        float sizeX, sizeY;
        sizeY = 2.0f * Mathf.Abs(transform.position.z) * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        sizeX = sizeY * _camera.aspect;
        _cameraBox.size = new Vector2(sizeX, sizeY);
    }
}
