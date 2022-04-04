using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CheckMethod
{
    Distance,
    Trigger,
    Neighbours
}

public class SceneLoader : MonoBehaviour
{
    public Transform player;
    public CheckMethod checkMethod;
    public float loadRange;
    public List<string> neighbours;

    private bool _isLoaded;
    private bool _shouldLoad;

    private void Update()
    {
        switch (checkMethod)
        {
            case CheckMethod.Distance:
                DistanceCheck();
                break;
            case CheckMethod.Trigger:
                break;
            case CheckMethod.Neighbours:
                NeighboursCheck();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DistanceCheck()
    {
        if (Vector3.Distance(player.position, transform.position) < loadRange)
        {
            LoadScene();
        }
        else
        {
            UnloadScene();
        }
    }

    private void NeighboursCheck()
    {
        if (CameraManager.Instance.currentRoom == null) return;
        
        GameObject currentRoomLoader = GameObject.Find(CameraManager.Instance.currentRoom);
        if (!currentRoomLoader.TryGetComponent(out SceneLoader sceneLoader)) return;
        
        if (sceneLoader.neighbours.Contains(gameObject.name) || gameObject.name == currentRoomLoader.name)
        {
            LoadScene();
        }
        else
        {
            UnloadScene();
        }
    }

    private void LoadScene()
    {
        if (_isLoaded) return;
        SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
        _isLoaded = true;
    }

    private void UnloadScene()
    {
        if (!_isLoaded) return;
        SceneManager.UnloadSceneAsync(gameObject.name);
        _isLoaded = false;
    }
}
