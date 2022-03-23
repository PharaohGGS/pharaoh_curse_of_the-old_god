using Cinemachine;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Managers;
using UnityEngine;

public class VirtualCameraHandler : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = CameraManager.Instance.player.GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        if (!TryGetComponent(out _virtualCamera))
            Debug.Log("No CinemachineVirtualCamera component found.");
        
        _virtualCamera.Follow = CameraManager.Instance.vcamFollowOffset.transform;
    }

    private void LateUpdate()
    {
        CameraManager.Instance.vcamFollowOffset.transform.position =
            CameraManager.Instance.player.transform.position + CameraManager.Instance.cameraOffset * (_playerMovement.IsFacingRight ? 1 : -1);
    }
}
