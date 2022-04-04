using UnityEngine;
using UnityEngine.SceneManagement;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;

public class PauseMenu: MonoBehaviour
{


    public static bool isGamePaused = false;
    public InputReader inputReader;
    public GameObject pausePanel;

    private void OnEnable()
    {
        inputReader.exitPerformedEvent += OnPauseMenu;
    }

    private void OnDisable()
    {
        inputReader.exitPerformedEvent -= OnPauseMenu;
    }

    public void OnPauseMenu()
    {
        if (isGamePaused)
            UnpauseGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);

        isGamePaused = true;
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        isGamePaused = false;
    }

    public void Continue()
    {
        UnpauseGame();
    }

    public void Save()
    {
        SaveDataManager.Instance.Save();
    }

    public void MainMenu()
    {
        UnpauseGame();
        SceneManager.LoadScene(0);
    }

}
