using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject virtualCamera;

    private void OnTriggerEnter2D(Collider2D col)
    {
        virtualCamera.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        virtualCamera.SetActive(false);
    }
    
    // private void OnTriggerEnter2D(Collider2D col)
    // {
    //     if (col.gameObject.CompareTag("Player") && col.gameObject.TryGetComponent(out Player player))
    //     {
    //         CameraManager.Instance.CurrentRoom = transform;
    //         GameObject.Find("VC_Base").GetComponent<CinemachineConfiner2D>().m_BoundingShape2D =
    //             GetComponent<PolygonCollider2D>();
    //     }
    // }
    //
    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (!other.gameObject.CompareTag("Player")) return;
    //     Debug.Log(gameObject.name);
    //     foreach (var obj in FindObjectsOfType(typeof(Enemy)))
    //     {
    //         var enemy = (Enemy) obj;
    //         if (gameObject.name == enemy.gameObject.scene.name)
    //         {
    //             SaveManager.SaveEnemy(enemy);
    //             enemy.Reset();
    //         }
    //     }
    // }
}
