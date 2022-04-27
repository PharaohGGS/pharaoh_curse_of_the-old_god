using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public List<string> neighbours; // List of room's neighbours

    private bool _isLoaded; // Is this room loaded or not
    private GameObject _currentRoom;
    private SceneLoader _currentSceneLoader;
    private bool _isCurrentRoomValid;

    private Coroutine _loadingCoroutine;
    private Coroutine _unloadingCoroutine;

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

    private void Update()
    {
        if (LevelManager.Instance == null) return;
        if (_currentSceneLoader == null) return;
        if (gameObject.name == _currentRoom.name)
        {
            if (_isLoaded) return;
            _isLoaded = true;
            if (_loadingCoroutine == null)
                SceneManager.LoadScene(gameObject.name, LoadSceneMode.Additive);
        }
        else if (_currentSceneLoader.neighbours.Contains(gameObject.name))
        {
            if (_isLoaded) return;
            if (_loadingCoroutine == null)
                _loadingCoroutine = StartCoroutine(LoadScene());
        }
        else
        {
            if (!_isLoaded) return;
            if (_unloadingCoroutine == null)
                _unloadingCoroutine = StartCoroutine(UnloadScene());
        }
    }

    private void OnRoomChanged()
    {
        _currentRoom = GameObject.Find(LevelManager.Instance.currentRoom);
        if (!_currentRoom.TryGetComponent(out _currentSceneLoader))
            _currentSceneLoader = null;
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            yield return null;
        }
        _isLoaded = true;
        _loadingCoroutine = null;
    }

    private IEnumerator UnloadScene()
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(gameObject.name);
        while (!operation.isDone)
        {
            yield return null;
        }
        _isLoaded = false;
        _unloadingCoroutine = null;
    }
}