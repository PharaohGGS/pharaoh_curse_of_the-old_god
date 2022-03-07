using UnityEngine;

public class MainMenu : MonoBehaviour
{

    private bool _isMainMenuDisplayed = true;
    private GameObject _mainMenu;
    private GameObject _settingsMenu;

    private void Awake()
    {
        _mainMenu = transform.Find("Main Menu").gameObject;
        _settingsMenu = transform.Find("Settings Menu").gameObject;
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

}
