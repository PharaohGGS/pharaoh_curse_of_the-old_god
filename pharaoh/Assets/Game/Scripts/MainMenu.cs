using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;

public class MainMenu : MonoBehaviour
{

    private readonly int[] SCREEN_SIZES = new int[] { 3840, 2160, 2560, 1440, 1920, 1080, 1600, 900, 1366, 768 };
    private readonly int DEFAULT_SCREEN_SIZE = 4;
    private readonly int DEFAULT_WINDOW_MODE = 0;
    private readonly int PREFERRED_REFRESH_RATE = 60;

    private bool _isMainMenuDisplayed = true;
    private GameObject _mainMenu;
    private GameObject _settingsMenu;
    private Button _continueButton;

    public TMPro.TMP_Dropdown windowModeDropdown;
    public TMPro.TMP_Dropdown resolutionDropdown;
    
    private void Start()
    {
        _mainMenu = transform.Find("Main Menu").gameObject;
        _settingsMenu = transform.Find("Settings Menu").gameObject;
        _continueButton = _mainMenu.transform.Find("Continue Button").GetComponent<Button>();

        UpdateMenus();
        UpdateScreenSizeAndWindowMode();
    }

    public void NewGame()
    {
        SaveDataManager.Instance.NewSave();
        LoadGameScene();
    }

    public void ContinueGame()
    {
        SaveDataManager.Instance.LoadSave();
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        Debug.Log("Loading Game Scene");
        SceneManager.LoadScene(1);
    }

    public void EraseSaveFile()
    {
        SaveDataManager.Instance.EraseSave();
        UpdateMenus();
    }

    public void SwitchMenu()
    {
        _isMainMenuDisplayed = !_isMainMenuDisplayed;

        UpdateMenus();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void UpdateMenus()
    {
        if (_isMainMenuDisplayed)
        {
            _mainMenu.SetActive(true);
            _settingsMenu.SetActive(false);
        }
        else
        {
            _mainMenu.SetActive(false);
            _settingsMenu.SetActive(true);
        }

        _continueButton.interactable = SaveDataManager.Instance.SaveFileExists();
    }

    public void UpdateScreenSizeAndWindowMode()
    {
        Screen.SetResolution(SCREEN_SIZES[resolutionDropdown.value * 2], SCREEN_SIZES[(resolutionDropdown.value * 2) + 1], windowModeDropdown.value == 0, PREFERRED_REFRESH_RATE);
    }

    public void ResetSettings()
    {
        windowModeDropdown.value = DEFAULT_WINDOW_MODE;
        resolutionDropdown.value = DEFAULT_SCREEN_SIZE;
        UpdateScreenSizeAndWindowMode();
    }

}
