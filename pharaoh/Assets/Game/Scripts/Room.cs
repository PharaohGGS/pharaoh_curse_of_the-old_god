using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject virtualCamera;
    public SpriteRenderer hider;

    private float _timer;
    private Coroutine _fading;

    private void OnTriggerEnter2D(Collider2D col)
    {
        virtualCamera.SetActive(true);
        // if (_fading != null) StopCoroutine(_fading);
        // _fading = StartCoroutine(Fade(0f, .3f));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        virtualCamera.SetActive(false);
        // if (_fading != null) StopCoroutine(_fading);
        // _fading = StartCoroutine(Fade(1f, .3f));
    }

    private IEnumerator Fade(float alpha, float time)
    {
        Color hiderColor = hider.color;
        float startAlpha = hiderColor.a;
        float progress = startAlpha < alpha ? (startAlpha / alpha) : (alpha / startAlpha);
        float timeToFade = time - time * progress;
        float elapsedTime = 0f;
        while (elapsedTime < timeToFade)
        {
            hiderColor.a = Mathf.Lerp(startAlpha, alpha, elapsedTime / timeToFade);
            hider.color = hiderColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hiderColor.a = alpha;
        hider.color = hiderColor;
        yield return null;
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
