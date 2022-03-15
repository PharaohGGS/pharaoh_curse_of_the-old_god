using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwords : MonoBehaviour
{

    private bool _sheathed = true;
    private PlayerInput _playerInput;

    public Animator animator;
    public Transform rightHand;
    public Transform leftHand;
    public Transform rightSocket;
    public Transform leftSocket;
    public Transform rightSword;
    public Transform leftSword;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.CharacterActions.SwitchStance.performed += SwitchStance;
    }

    private void SwitchStance(InputAction.CallbackContext ctx)
    {
        Debug.Log("Switching Stance");

        if (_sheathed)
            animator.SetTrigger("Draw");
        else
            animator.SetTrigger("Sheathe");

        _sheathed = !_sheathed;
    }

    private void DrawSwords()
    {
        rightSword.parent = rightHand;
        rightSword.localPosition = Vector3.zero;
        rightSword.localRotation = Quaternion.identity;
        leftSword.parent = leftHand;
        leftSword.localPosition = Vector3.zero;
        leftSword.localRotation = Quaternion.identity;
    }

    private void SheatheSwords()
    {
        rightSword.parent = rightSocket;
        rightSword.localPosition = Vector3.zero;
        rightSword.localRotation = Quaternion.identity;
        leftSword.parent = leftSocket;
        leftSword.localPosition = Vector3.zero;
        leftSword.localRotation = Quaternion.identity;
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
