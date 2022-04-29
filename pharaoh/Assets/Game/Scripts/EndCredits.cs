using UnityEngine;
using UnityEngine.SceneManagement;
using AudioManager = Pharaoh.Managers.AudioManager;

public class EndCredits : MonoBehaviour
{

    public InputReader inputReader;

    private void OnEnable()
    {
        inputReader.exitPerformedEvent += TheEnd;
    }

    private void OnDisable()
    {
        inputReader.exitPerformedEvent -= TheEnd;
    }

    public void TheEnd()
    {
        // Love you all, it's been such a great adventure and I really hope y'all do great afterwards
        // except that one fucker, of course
        AudioManager.Instance?.StopAllMusic();
        SceneManager.LoadScene("MainMenu");
    }

}
