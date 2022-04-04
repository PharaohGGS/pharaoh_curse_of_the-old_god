using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Pharaoh.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public GameObject virtualCamera;
    public SpriteRenderer hider;

    private float _timer;
    private Coroutine _fading;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player") && col.gameObject.layer != LayerMask.NameToLayer("Player - Swarm")) return;

        virtualCamera.SetActive(true);
        CameraManager.Instance.currentRoom = gameObject.scene.name;
        Debug.Log(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") && other.gameObject.layer != LayerMask.NameToLayer("Player - Swarm")) return;

        virtualCamera.SetActive(false);
    }
}
