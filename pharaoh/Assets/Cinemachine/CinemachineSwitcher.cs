using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineSwitcher : MonoBehaviour
{
    
    [SerializeField]
    private InputAction action;
    
    [SerializeField][Tooltip("Base Virtual Camera")]
    private CinemachineVirtualCamera vcam1;
    
    [SerializeField][Tooltip("PoI Virtual Camera")]
    private CinemachineVirtualCamera vcam2; // 

    private bool _baseCamera;

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        action.performed += _ => SwitchPriority();
    }

    private void SwitchPriority()
    {
        vcam1.Priority = _baseCamera ? 1 : 0;
        vcam2.Priority = _baseCamera ? 0 : 1;
        _baseCamera = !_baseCamera;
    }
}
