using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerSkills : MonoBehaviour
{
    
    public InputAction action;
    public LayerMask layers;
    public float teleportMinusDelay = 0f;
    
    private VisualEffect _teleportVFX;
    private PlayerMovement _playerMovement;

    public GameObject[] meshes;


    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    void Start()
    {
        if (TryGetComponent(out VisualEffect vfx))
            _teleportVFX = vfx;
        if (TryGetComponent(out PlayerMovement playerMovement))
            _playerMovement = playerMovement;
        action.performed += _ => Teleport();
    }

    private void Teleport()
    {
        StartCoroutine(TeleportCoroutine());
    }

    private IEnumerator TeleportCoroutine()
    {
        action.Disable();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        RigidbodyConstraints2D initialConstraints = rb.constraints;
        rb.constraints = initialConstraints | RigidbodyConstraints2D.FreezePosition;

        foreach (var mesh in meshes) { mesh.SetActive(false); }
        
        Vector3 target = _teleportVFX.GetVector3("TargetPosition");
        float z = transform.Find("Model").localScale.z;
        if (Math.Abs(Mathf.Sign(target.z) - Mathf.Sign(z)) > 0.1f)
            target *= -1;
        _teleportVFX.SetVector3("TargetPosition", target);
        
        _teleportVFX.Play();

        float[] delays =
        {
            _teleportVFX.GetFloat("DissolveLifetime"),
            _teleportVFX.GetFloat("DashLifetime"),
            _teleportVFX.GetFloat("RespawnLifetime")
        };
        yield return new WaitForSeconds(delays.Sum() - teleportMinusDelay);

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            new Vector2(target.z, target.y),
            target.magnitude,
            layers);

        if (hit)
            transform.position = hit.point;
        else
            transform.position += new Vector3(target.z, target.y, target.x);
        
        foreach (var mesh in meshes) { mesh.SetActive(true); }
        rb.constraints = initialConstraints;
        action.Enable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(3 * transform.Find("Model").localScale.z, 0, 0));
    }
}
