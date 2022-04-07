using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;

public class PauseMenu: MonoBehaviour
{

    [Header("Menu displaying")]
    public static bool isGamePaused = false;
    public InputReader inputReader;
    public GameObject pausePanel;
    [Space(10)]
    [Header("Skills displaying")]
    public PlayerSkills playerSkills;
    public List<GameObject> displayedSkills;

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

        DisplaySkills();
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

    public void DisplaySkills()
    {
        Debug.Log("Displaying skills");
        GameObject unlocked0 = displayedSkills[0];
        GameObject unlocked1 = displayedSkills[1];
       /* GameObject unlocked2 = displayedSkills[2];
        GameObject unlocked3 = displayedSkills[3];
        GameObject unlocked4 = displayedSkills[4];*/

        unlocked0.SetActive(playerSkills.hasCanopicJar1);
        unlocked1.SetActive(playerSkills.hasCanopicJar2);
        /*unlocked2.SetActive(playerSkills.hasCanopicJar2);
        unlocked2.SetActive(playerSkills.hasCanopicJar2);
        unlocked2.SetActive(playerSkills.hasCanopicJar2);*/
    }

}
