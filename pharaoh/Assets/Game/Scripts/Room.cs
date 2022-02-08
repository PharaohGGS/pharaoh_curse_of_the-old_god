using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Room : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && col.gameObject.TryGetComponent(out Player player))
        {
            CameraManager.Instance.CurrentRoom = transform;
        }
    }
}
