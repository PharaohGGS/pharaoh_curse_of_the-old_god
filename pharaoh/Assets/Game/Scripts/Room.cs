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
        if (col.gameObject.layer != LayerMask.NameToLayer("Player")
            && col.gameObject.layer != LayerMask.NameToLayer("Player - Dash")
            && col.gameObject.layer != LayerMask.NameToLayer("Player - Swarm")) return;

        virtualCamera.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (LevelManager.Instance == null) return;
        if (col.gameObject.layer != LayerMask.NameToLayer("Player")
            && col.gameObject.layer != LayerMask.NameToLayer("Player - Dash")
            && col.gameObject.layer != LayerMask.NameToLayer("Player - Swarm")) return;
        
        if (LevelManager.Instance.currentRoom != gameObject.scene.name)
            LevelManager.Instance.ChangeRoom(gameObject.scene.name);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        
        if (col.gameObject.layer != LayerMask.NameToLayer("Player")
            && col.gameObject.layer != LayerMask.NameToLayer("Player - Dash")
            && col.gameObject.layer != LayerMask.NameToLayer("Player - Swarm")) return;

        virtualCamera.SetActive(false);
        Debug.Log("EXIT");
    }
}
