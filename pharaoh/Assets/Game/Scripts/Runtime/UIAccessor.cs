using UnityEngine;

public class UIAccessor : MonoBehaviour
{

    public static PauseMenu pauseMenu;
    public static GameObject loadingScreen;

    private void Awake()
    {
        pauseMenu = transform.Find("PauseMenu").GetComponent<PauseMenu>();
        loadingScreen = transform.Find("LoadingScreen").gameObject;
    }

}
