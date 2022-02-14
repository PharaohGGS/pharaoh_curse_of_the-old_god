using System;
using Cinemachine;
using Pharaoh.Gameplay.Component;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 _movement;
    private Rigidbody2D _rigidbody;

    public Camera cam1;
    public Camera cam2;
    public CinemachineVirtualCamera virtualCamera;
    public Transform target;

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

    public void OnFire()
    {
        // virtualCamera.transform.position = cam1.transform.position;
        // virtualCamera.transform.rotation = cam1.transform.rotation;
        //
        // cam1.enabled = !cam1.enabled;
        // cam2.enabled = !cam2.enabled;
        //
        // virtualCamera.LookAt = cam2.enabled ? target.transform : null;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (TryGetComponent(out DamageComponent damageComponent))
        {
            damageComponent.Hit(col.gameObject);
        }
    }
}
