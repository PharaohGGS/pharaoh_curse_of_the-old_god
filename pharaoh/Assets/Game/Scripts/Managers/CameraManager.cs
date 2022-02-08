using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Pharaoh.Tools;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    public delegate void RoomChange();
    public event RoomChange OnRoomChange;
    
    private Transform _currentRoom;
    public Transform CurrentRoom
    {
        get => _currentRoom;
        set
        {
            _currentRoom = value;
            OnRoomChange?.Invoke();
        }
    }

    private void OnEnable()
    {
        OnRoomChange += ChangeRoom;
    }

    private void OnDisable()
    {
        OnRoomChange -= ChangeRoom;
    }

    private void ChangeRoom()
    {
        if (TryGetComponent(out CinemachineVirtualCamera virtualCamera))
        {
            //virtualCamera.Follow = CurrentRoom;
            if (TryGetComponent(out CinemachineConfiner2D confiner) &&
                CurrentRoom.TryGetComponent(out PolygonCollider2D polygonCollider2D))
            {
                confiner.m_BoundingShape2D = polygonCollider2D;
            }
        }
    }
}
