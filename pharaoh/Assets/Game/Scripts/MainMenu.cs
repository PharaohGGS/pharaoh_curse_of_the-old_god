using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject loadingUI;
    public Image loadingProgressBar;

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    public void StartGame()
    {
        ToggleMenu();
        ToggleLoadingUI();
        StartCoroutine(LoadingScreen());
    }

    IEnumerator LoadingScreen()
    {
        float progress = 0f;
        foreach (var op in _scenesToLoad)
        {
            while (!op.isDone)
            {
                progress += op.progress;
                loadingProgressBar.fillAmount = progress / _scenesToLoad.Count;
                yield return null;
            }
        }
        ToggleLoadingUI();
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }
    
    private void ToggleLoadingUI()
    {
        loadingUI.SetActive(!loadingUI.activeSelf);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
