using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuInput : MonoBehaviour
{

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Game.MainMenu.performed += OnQuitGame;
    }

    private void OnQuitGame(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(0);
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

}
