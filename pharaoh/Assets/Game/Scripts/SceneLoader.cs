using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public UnityEngine.Object scene;
    public List<UnityEngine.Object> neighbours; // List of room's neighbours

    private bool _isLoaded; // Is this room loaded or not

    private void Start()
    {
        OnRoomChanged();
    }

    private void OnEnable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.roomChanged += OnRoomChanged;
        }
    }

    private void OnDisable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.roomChanged -= OnRoomChanged;
        }
    }

    private void OnRoomChanged()
    {
        Debug.Log("1");
        if (LevelManager.Instance == null)
            return;
        Debug.Log("2");
        // Get the GameObject which represents the current room
        GameObject currentRoom = GameObject.Find(LevelManager.Instance.currentRoom);
        Debug.Log("3");
        if (!currentRoom.TryGetComponent(out SceneLoader sceneLoader)) return; // If it has no SceneLoader, return
        Debug.Log(scene.name);
        Debug.Log("4");
        Debug.Log(currentRoom.name);
        // If this room is in CurrentRoom's neighbours or is the CurrentRoom, load it
        if (scene.name == currentRoom.name)
        {
            if (_isLoaded) return;
            _isLoaded = true;
            Debug.Log("5");
            SceneManager.LoadScene(scene.name, LoadSceneMode.Additive);
            Debug.Log("6");
        }
        else if (sceneLoader.neighbours.Contains(scene))
        {
            if (_isLoaded) return;
            _isLoaded = true;
            Debug.Log("7");
            StartCoroutine(LoadScene());
            Debug.Log("8");
        }
        else
        {
            if (!_isLoaded) return;
            _isLoaded = false;
            Debug.Log("9");
            StartCoroutine(UnloadScene());
            Debug.Log("10");
        }
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            yield return null;
        }
    }
    
    private IEnumerator UnloadScene()
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(scene.name);
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
