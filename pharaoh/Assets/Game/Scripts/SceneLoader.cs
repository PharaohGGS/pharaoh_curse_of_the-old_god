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

    private void Start()
    {
        
    }

    private void Update()
    {
        if (checkMethod == CheckMethod.Distance)
        {
            DistanceCheck();
        }
        else if (checkMethod == CheckMethod.Trigger)
        {
            
        }
        else if (checkMethod == CheckMethod.Neighbours)
        {
            NeighboursCheck();
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

    public void LoadScene()
    {
        if (!_isLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            _isLoaded = true;
        }
    }

    public void UnloadScene()
    {
        if (_isLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            _isLoaded = false;
        }
    }
}
