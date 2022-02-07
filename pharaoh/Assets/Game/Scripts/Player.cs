using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private Vector2 _movement;
    private Rigidbody2D _rigidbody;
    public SceneLoader CurrentSceneLoader { get; set; }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _movement;
    }

    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        _movement = new Vector3(inputVec.x, inputVec.y, 0);
    }
    
    public void ChangeRoom(Transform room)
    {
        virtualCamera.Follow = room;
    }
}
