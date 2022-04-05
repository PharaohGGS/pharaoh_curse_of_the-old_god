using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public List<string> neighbours; // List of room's neighbours

    private bool _isLoaded; // Is this room loaded or not

    private void Update()
    {
        if (LevelManager.Instance.currentRoom == null) return;
        
        // Get the GameObject which represents the current room
        GameObject currentRoom = GameObject.Find(LevelManager.Instance.currentRoom);
        
        if (!currentRoom.TryGetComponent(out SceneLoader sceneLoader)) return; // If it has no SceneLoader, return
        
        // If this room is in CurrentRoom's neighbours or is the CurrentRoom, load it
        if (sceneLoader.neighbours.Contains(gameObject.name) || gameObject.name == currentRoom.name)
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
