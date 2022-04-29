using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("MainMenu");
    }

}
