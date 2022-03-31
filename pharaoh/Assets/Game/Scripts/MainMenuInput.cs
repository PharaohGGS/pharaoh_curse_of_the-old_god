using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuInput : MonoBehaviour
{

    public InputReader inputReader;

    private void Awake()
    {
        inputReader.exitPerformedEvent += OnExitGame;
    }

    private void OnExitGame()
    {
        Debug.Log("Back to Main Menu");
        SceneManager.LoadScene(0);
    }

}
