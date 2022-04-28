using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;
using AudioManager = Pharaoh.Managers.AudioManager;

public class MainMenu : MonoBehaviour
{

    private readonly int[] SCREEN_SIZES = new int[] { 3840, 2160, 2560, 1440, 1920, 1080, 1600, 900, 1366, 768 };
    private readonly int DEFAULT_SCREEN_SIZE = 4;
    private readonly int DEFAULT_WINDOW_MODE = 0;
    private readonly int PREFERRED_REFRESH_RATE = 60;

    private bool _isMainMenuDisplayed = true;
    private GameObject _mainMenu;
    private GameObject _settingsMenu;
    private GameObject _continueButton;

    public TMPro.TMP_Dropdown windowModeDropdown;
    public TMPro.TMP_Dropdown resolutionDropdown;
    
    private void Start()
    {
        AudioManager.Instance.Play("Menu");
        AudioManager.Instance.Play("MenuAmbiance");

        _mainMenu = transform.Find("Main Menu").gameObject;
        _settingsMenu = transform.Find("Settings Menu").gameObject;
        _continueButton = _mainMenu.transform.Find("Continue Button").gameObject;

        if (SaveDataManager.Instance.PrefsFileExists())
        {
            SaveDataManager.Instance.LoadPrefsFromJSON();
            windowModeDropdown.value = SaveDataManager.Instance.LoadWindowMode();
            resolutionDropdown.value = SaveDataManager.Instance.LoadResolutionValue();
        }

        UpdateMenus();
        UpdateScreenSizeAndWindowMode();
    }

    public void ContinueGame()
    {
        AudioManager.Instance.Stop("Menu");
        AudioManager.Instance.Stop("MenuAmbiance");
        SaveDataManager.Instance.LoadSave();
        LoadGameScene();
    }

    public void NewGame()
    {
        AudioManager.Instance.Stop("Menu");
        AudioManager.Instance.Stop("MenuAmbiance");
        SaveDataManager.Instance.NewSave();
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

        bool interactable = SaveDataManager.Instance.SaveFileExists();
        _continueButton.GetComponent<Button>().interactable = interactable;
        _continueButton.GetComponent<HoverButton>().interactable = interactable;
        _continueButton.GetComponent<HoverButton>().UpdateDisplay();
    }

    public void UpdateScreenSizeAndWindowMode()
    {
        Screen.SetResolution(SCREEN_SIZES[resolutionDropdown.value * 2], SCREEN_SIZES[(resolutionDropdown.value * 2) + 1], windowModeDropdown.value == 0, PREFERRED_REFRESH_RATE);

        SaveDataManager.Instance.SavePrefs(windowModeDropdown.value, resolutionDropdown.value);
    }

    public void ResetSettings()
    {
        windowModeDropdown.value = DEFAULT_WINDOW_MODE;
        resolutionDropdown.value = DEFAULT_SCREEN_SIZE;
        UpdateScreenSizeAndWindowMode();
    }

}
