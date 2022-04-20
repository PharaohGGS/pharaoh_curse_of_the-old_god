using Cinemachine;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Managers;
using UnityEngine;

public class VirtualCameraHandler : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private PlayerMovement _playerMovement;

    private bool _isOnTrack;
    private PolygonCollider2D _polygonCollider2D;
    private CinemachineTrackedDolly _trackedDolly;

    private void Awake()
    {
        _playerMovement = CameraManager.Instance.player.GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        if (!TryGetComponent(out _virtualCamera))
            Debug.Log("No CinemachineVirtualCamera component found.");
        
        _virtualCamera.Follow = CameraManager.Instance.vcamFollowOffset.transform;
        _trackedDolly = _virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        if (_virtualCamera.GetCinemachineComponent<CinemachineTransposer>() == null && _trackedDolly == null)
            _virtualCamera.LookAt = CameraManager.Instance.player.transform;
        if (_trackedDolly != null)
        {
            _isOnTrack = true;
            _polygonCollider2D = _virtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D as PolygonCollider2D;
        }

    }

    private void LateUpdate()
    {
        CameraManager.Instance.vcamFollowOffset.transform.position =
            (CameraManager.Instance.player.transform.position + Vector3.up) + CameraManager.Instance.cameraOffset * (_playerMovement.IsFacingRight ? 1 : -1);

        if (_isOnTrack)
        {
            Debug.Log(_polygonCollider2D.bounds.max);
            _trackedDolly.m_PathPosition =
                (CameraManager.Instance.vcamFollowOffset.transform.position.x - _polygonCollider2D.bounds.min.x) /
                (_polygonCollider2D.bounds.max.x - _polygonCollider2D.bounds.min.x);
        }
    }
}
