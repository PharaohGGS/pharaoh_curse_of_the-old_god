using UnityEngine;

public class MainMenu : MonoBehaviour
{

    private int _preferredRefreshRate = 60;
    private bool _isMainMenuDisplayed = true;
    private GameObject _mainMenu;
    private GameObject _settingsMenu;
    private readonly int[] _screenSizes = new int[] { 3840, 2160, 2560, 1440, 1920, 1080, 1600, 900, 1366, 768 };

    public TMPro.TMP_Dropdown windowModeDropdown;
    public TMPro.TMP_Dropdown resolutionDropdown;

    private void Awake()
    {
        _mainMenu = transform.Find("Main Menu").gameObject;
        _settingsMenu = transform.Find("Settings Menu").gameObject;

        UpdateMenus();
    }

    public void SwitchMenu()
    {
        _isMainMenuDisplayed = !_isMainMenuDisplayed;

        UpdateMenus();
    }

    private void UpdateMenus()
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
    }

    public void UpdateScreenSizeAndWindowMode()
    {
        Debug.Log("Changing resolution to " + _screenSizes[resolutionDropdown.value * 2] + " x " + _screenSizes[(resolutionDropdown.value * 2) + 1] + " " + (windowModeDropdown.value == 0));
        Screen.SetResolution(_screenSizes[resolutionDropdown.value * 2], _screenSizes[(resolutionDropdown.value * 2) + 1], windowModeDropdown.value == 0, _preferredRefreshRate);
        Debug.Log("Screen changed to " + Screen.currentResolution + " " + Screen.fullScreen);
    }

}
