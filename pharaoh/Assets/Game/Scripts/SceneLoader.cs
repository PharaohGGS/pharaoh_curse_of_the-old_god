using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public List<string> neighbours; // List of room's neighbours

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
        if (LevelManager.Instance == null)
            return;

        // Get the GameObject which represents the current room
        GameObject currentRoom = GameObject.Find(LevelManager.Instance.currentRoom);

        if (!currentRoom.TryGetComponent(out SceneLoader sceneLoader)) return; // If it has no SceneLoader, return

        // If this room is in CurrentRoom's neighbours or is the CurrentRoom, load it
        if (gameObject.name == currentRoom.name)
        {
            if (_isLoaded) return;
            _isLoaded = true;
            UIAccessor.loadingScreen.SetActive(true); // Display loading screen
            Debug.Log("Loading");
            SceneManager.LoadScene(gameObject.name, LoadSceneMode.Additive);
            UIAccessor.loadingScreen.SetActive(false); // Hide loading screen
        }
        else if (sceneLoader.neighbours.Contains(gameObject.name))
        {
            if (_isLoaded) return;
            _isLoaded = true;
            StartCoroutine(LoadScene());
        }
        else
        {
            if (!_isLoaded) return;
            _isLoaded = false;
            StartCoroutine(UnloadScene());
        }
    }

    private IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator UnloadScene()
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(gameObject.name);
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}