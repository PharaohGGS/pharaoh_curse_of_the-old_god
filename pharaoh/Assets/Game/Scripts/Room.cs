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
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player") && col.gameObject.layer != LayerMask.NameToLayer("Player - Swarm")) return;

        virtualCamera.SetActive(true);

        if (!LevelManager.Instance.debugging)
            LevelManager.Instance.currentRoom = gameObject.scene.name;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") && other.gameObject.layer != LayerMask.NameToLayer("Player - Swarm")) return;

        virtualCamera.SetActive(false);
    }
}
