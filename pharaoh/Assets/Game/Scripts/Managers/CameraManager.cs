using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Pharaoh.Tools;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    private Transform _currentRoom;
    private Bounds _currentRoomBoundaries;
    
    public Transform CurrentRoom { get; set; }
    public Bounds CurrentRoomBoundaries
    {
        get => CurrentRoom.TryGetComponent(out BoxCollider2D boxCollider2D) ? boxCollider2D.bounds : new Bounds();
        private set => _currentRoomBoundaries = value;
    }
}
