using System;
using System.Collections;
using System.Collections.Generic;
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
    public List<SceneLoader> neighbours;

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
        if (player.TryGetComponent(out Player playerComponent) &&
            playerComponent.CurrentSceneLoader.neighbours.Contains(this) ||
            playerComponent.CurrentSceneLoader == this)
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
