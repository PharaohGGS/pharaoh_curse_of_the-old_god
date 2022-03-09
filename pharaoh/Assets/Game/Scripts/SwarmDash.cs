using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class SwarmDash : MonoBehaviour
{
    public LayerMask groundLayer;
    public float teleportMinusDelay = 0f;
    public Vector3 targetPosition;
    public VisualEffect teleportVFX;
    
    private PlayerMovement _playerMovement;
    private PlayerInput _playerInput;
    private float _facingSide;

    public GameObject[] meshes;
    
    [Header("References")]
    [Tooltip("Place the player's model reference, used to get player facing direction.")]
    public Transform playerModel;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.CharacterControls.SwarmDash.performed += Teleport;
        _playerInput.CharacterControls.Move.performed += SetSide;
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    void Start()
    {
        if (TryGetComponent(out VisualEffect vfx))
            teleportVFX = vfx;
        if (TryGetComponent(out PlayerMovement playerMovement))
            _playerMovement = playerMovement;
        _facingSide = ((playerModel.rotation.eulerAngles.y > 150) ? -1 : 1);
    }

    private void Teleport(InputAction.CallbackContext obj)
    {
        StartCoroutine(TeleportCoroutine());
    }

    private IEnumerator TeleportCoroutine()
    {
        teleportVFX.gameObject.transform.position = transform.position;
        
        _playerInput.Disable();
        Collider2D col = GetComponent<Collider2D>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        RigidbodyConstraints2D initialConstraints = rb.constraints;
        rb.constraints = initialConstraints | RigidbodyConstraints2D.FreezePosition;

        Vector3 extents = col.bounds.extents;

        foreach (var mesh in meshes) { mesh.SetActive(false); }
        col.enabled = false;

        Vector3 target = targetPosition;
        target *= _facingSide;
        teleportVFX.SetVector3("TargetPosition", target);

        teleportVFX.SetVector3("KillBoxSize", new Vector3(0, 0, 0));
        teleportVFX.Play();

        float[] delays =
        {
            teleportVFX.GetFloat("DissolveLifetime"),
            teleportVFX.GetFloat("DashLifetime"),
            teleportVFX.GetFloat("RespawnLifetime")
        };

        Vector3 start = transform.position;
        Vector3 end = start + target;
        
        float elapsed = 0f;
        while (elapsed < delays.Sum())
        {
            elapsed += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(start, end, elapsed / delays.Sum());
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position + new Vector3(extents.x * _facingSide, 0, 0),
                end - start,
                Vector3.Distance(transform.position, newPos),
                groundLayer);
            if (hit)
            {
                teleportVFX.SetVector3("KillBoxSize", new Vector3(50, 50, 50));
                teleportVFX.Stop();
                
                col.enabled = true;
                foreach (var mesh in meshes) { mesh.SetActive(true); }
                rb.constraints = initialConstraints;
                _playerInput.Enable();
                yield break;
            }
            transform.position = newPos;
            yield return null;
        }
        transform.position = end;
        //yield return new WaitForSeconds(delays.Sum() - teleportMinusDelay);

        col.enabled = true;
        foreach (var mesh in meshes) { mesh.SetActive(true); }
        rb.constraints = initialConstraints;
        _playerInput.Enable();
        yield return null;
    }

    private void SetSide(InputAction.CallbackContext obj)
    {
        _facingSide = Mathf.Sign(_playerInput.CharacterControls.Move.ReadValue<Vector2>().x);
    }

    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        Vector3 v = transform.position + new Vector3(col.bounds.extents.x * _facingSide, 0, 0);
        Gizmos.DrawSphere(v, 0.1f);
    }
}
