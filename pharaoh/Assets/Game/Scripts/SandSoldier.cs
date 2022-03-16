using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class SandSoldier : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Prefab for the final sand soldier.")]
    public GameObject sandSoldier;
    [Tooltip("Prefab for the soldier preview.")]
    public GameObject soldierPreview;
    
    [Header("References")]
    [Tooltip("Place the player's model reference, used to get player facing direction.")]
    public Transform playerModel;
    
    [Header("Parameters")]
    [Tooltip("Sand soldier minimum range from player.")]
    public float minRange = 1f;
    [Tooltip("Sand soldier maximum range from player.")]
    public float maxRange = 6f;
    [Tooltip("Pretty explicit, how much time the preview takes to go to the max range.")]
    public float timeToMaxRange = 1f;
    [Tooltip("How much time does it take from button release to fully grown soldier. A low value can create conflicts with other colliders.")]
    public float timeToAppearFromGround = 0.35f;
    [Tooltip("Soldier's life expectancy.")]
    public float timeToExpire = 10f;
    [Tooltip("Pick every layer which represent the ground, used to snap the objects to the ground.")]
    public LayerMask groundLayer;

    [Header("DEBUG")] public Vector3[] raycastHits;

    // [Header("VFX")]
    // [Tooltip("Soldier position preview VFX")]
    // public VisualEffect previewVFX;

    private PlayerInput _playerInput; // Input System
    private Coroutine _previewCoroutine; // Stores the preview coroutine
    private Coroutine _colliderCoroutine; // Useless to store this coroutine for now
    private Coroutine _expiredCoroutine; // Coroutine to destroy the soldier when its time.
    private GameObject _soldier; // Stores the reference of the final sand soldier
    private Vector3 _soldierPosition; // used to store the soldier position during the preview
    private bool _summoned; // boolean to check whether the soldier has already been summoned (to avoid conflicts)
    private RaycastHit2D _groundHit; // stores whether the current position is over a proper ground
    private GameObject _soldierPreviewInstance; // stores the instantiated preview prefab
    private PlayerMovement _playerMovement;
    private Vector3 _debugPosition = Vector3.zero;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.CharacterActions.SandSoldier.started += InitiateSummon; // Pressed
        //_playerInput.CharacterActions.SandSoldier.canceled += SummonSoldier; // Released
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.CharacterActions.SandSoldier.started -= InitiateSummon;
        //_playerInput.CharacterActions.SandSoldier.canceled -= SummonSoldier;
    }
    
    private void InitiateSummon(InputAction.CallbackContext obj)
    {
        if (_playerMovement.IsHookedToBlock) return;

        // Vector3 pos = transform.position;
        // pos += Vector3.right * (_playerMovement.IsFacingRight ? 1 : -1);
        // RaycastPlatforms(pos);
        StopAllCoroutines();
        StartCoroutine(Preview());
    }

    private IEnumerator Preview()
    {
        float finalX = transform.position.x + maxRange * (_playerMovement.isFacingRight ? 1 : -1);
        Vector2 previewPosition = transform.position;
        float elapsed = 0f;
        
        while (elapsed < timeToMaxRange)
        {
            elapsed += Time.deltaTime;
            previewPosition.x = Mathf.Lerp(transform.position.x, finalX, elapsed / timeToMaxRange);

            // Snap to ground
            RaycastHit2D groundHit = Physics2D.Raycast(previewPosition, Vector2.down, 50.0f, groundLayer);
            previewPosition.y = groundHit.point.y;
            
            // Find platforms
            RaycastHit2D[] results = RaycastPlatforms(previewPosition);
            
            // Detect if stuck in wall
            Collider2D col = Physics2D.OverlapCircle(previewPosition + new Vector2(0, 0.1f), 0f, groundLayer);
            if (col != null)
            {
                Debug.Log("stuck");
                previewPosition.y = results[1].point.y;
            }

            _debugPosition = previewPosition;
            
            yield return null;
        }

        yield return null;
    }

    private RaycastHit2D[] RaycastPlatforms(Vector2 source)
    {
        RaycastHit2D[] buffer = new RaycastHit2D[2];
        int hits = Physics2D.RaycastNonAlloc(source + new Vector2(0f, 0.1f), Vector2.up, buffer, 100.0f, groundLayer);

        RaycastHit2D[] results = new RaycastHit2D[hits];
        for (int i = 0; i < hits; i++)
        {
            Vector3 pos = buffer[i].point + Vector2.down;
            RaycastHit2D groundHit = Physics2D.Raycast(pos, Vector2.down, 100.0f, groundLayer);
            results[i] = groundHit;
        }
        
        raycastHits = new Vector3[hits];
        for (int i = 0; i < hits; i++)
        {
            raycastHits[i] = results[i].point;
        }
        return results;
    }

    // Called on button press
    // Cancel / Delete previous summons and start the preview
    // private void InitiateSummon(InputAction.CallbackContext obj)
    // {
    //     if (_playerMovement.IsHookedToBlock) return;
    //     
    //     StopAllCoroutines();
    //     _summoned = false;
    //     if (_soldier != null) // If there's already a sand soldier, destroy it
    //         Destroy(_soldier);
    //     if (_expiredCoroutine != null)
    //         StopCoroutine(_expiredCoroutine);
    //     
    //     // Calculate the start position of the preview from minRange and model rotation.
    //     Vector3 startPosition = transform.position + new Vector3(minRange, 0, 0) * (_playerMovement.isFacingRight ? 1 : -1);
    //     _soldierPreviewInstance = Instantiate(soldierPreview, startPosition, Quaternion.identity);
    //     _previewCoroutine = StartCoroutine(PreviewSoldier(startPosition));
    //     // previewVFX.SetVector3("KillBoxSize", Vector3.zero);
    // }

    // Called on button release or on other specific conditions
    // Instantiates the soldier and starts the growing collider coroutine
    private void SummonSoldier(InputAction.CallbackContext obj = new())
    {
        if (_playerMovement.IsHookedToBlock) return;
        
        if (_summoned) return;
        _summoned = true;

        if (_previewCoroutine != null)
            StopCoroutine(_previewCoroutine);
        _previewCoroutine = null;
        
        // previewVFX.SetVector3("KillBoxSize", new Vector3(50, 50, 50));
        // previewVFX.Stop();
        Destroy(_soldierPreviewInstance);
        
        if (!_groundHit) return;
        _soldierPosition.z = transform.position.z;
        _soldier = Instantiate(sandSoldier, _soldierPosition, Quaternion.identity);
        _colliderCoroutine = StartCoroutine(MoveSoldierCollider(_soldier.GetComponent<BoxCollider2D>()));
    }

    // Lerp from minRange to maxRange to preview the soldier position at time t
    // Check if ground is below and if a wall is in the way
    private IEnumerator PreviewSoldier(Vector3 startPosition)
    {
        // VFX
        // previewVFX.gameObject.transform.position = transform.position;
        // previewVFX.Play();
        float playerSize = 1.75f;

        RaycastHit2D wallHit;
        wallHit = Physics2D.Raycast(
            new Vector2(transform.position.x, startPosition.y + playerSize / 2f),
            _playerMovement.isFacingRight ? Vector2.right : Vector2.left,
            minRange,
            groundLayer);
        if (wallHit)
        {
            Debug.Log(wallHit.point);
            _soldierPosition.x = wallHit.point.x -
                                 sandSoldier.GetComponent<BoxCollider2D>().size.x / 2f *
                                 sandSoldier.transform.localScale.x *
                                 (_playerMovement.isFacingRight ? 1 : -1);
            _groundHit = Physics2D.Raycast(new Vector2(_soldierPosition.x, wallHit.point.y), Vector2.down, 10f, groundLayer);
            _soldierPosition.y = _groundHit.point.y +
                                 sandSoldier.GetComponent<BoxCollider2D>().size.y / 2f *
                                 sandSoldier.transform.localScale.y;
            SummonSoldier();
            yield break;
        }

        _soldierPosition = startPosition;
        float endX = startPosition.x + (maxRange - minRange) * (_playerMovement.isFacingRight ? 1 : -1);
        
        
        float elapsed = 0f;
        while (elapsed < timeToMaxRange)
        {
            elapsed += Time.deltaTime;
            
            float newX = Mathf.Lerp(startPosition.x, endX, elapsed / timeToMaxRange);

            Vector3 raycastPos = new Vector3(newX, _soldierPosition.y + playerSize / 2f, startPosition.z);

            _groundHit = Physics2D.Raycast(raycastPos, Vector2.down, 10f, groundLayer);
            wallHit = Physics2D.Raycast(
                raycastPos,
                _playerMovement.isFacingRight ? Vector2.right : Vector2.left,
                sandSoldier.transform.localScale.x / 2f,
                groundLayer);

            if (_groundHit)
                _soldierPosition = _groundHit.point + new Vector2(0,
                    sandSoldier.GetComponent<BoxCollider2D>().size.y / 2f * sandSoldier.transform.localScale.y);
            else
                _soldierPosition = new Vector3(newX, _soldierPosition.y);
            
            if (wallHit)
            {
                SummonSoldier();
                yield break;
            }

            // Vector3 vfxPos = _soldierPosition - previewVFX.gameObject.transform.position;
            // previewVFX.SetVector3("TargetPosition", vfxPos);
            _soldierPreviewInstance.transform.position = _soldierPosition;

            yield return null;
        }
        // previewVFX.SetVector3("TargetPosition", endPosition - transform.position);
        _soldierPosition = new Vector3(endX, _soldierPosition.y);
        SummonSoldier();
        yield return null;
    }

    // Lerp the soldier collider size and offset from 0 to full size
    // Used to lift object
    private IEnumerator MoveSoldierCollider(BoxCollider2D col)
    {
        float elapsed = 0f;
        col.offset = new Vector2(0, -0.5f);
        col.size = new Vector2(1, 0);
        _soldier.GetComponent<MeshRenderer>().enabled = false;
        while (elapsed < timeToAppearFromGround)
        {
            Vector2 offset = col.offset;
            Vector2 size = col.size;
            offset.y = Mathf.Lerp(-0.5f, 0, elapsed / timeToAppearFromGround);
            size.y = Mathf.Lerp(0, 1, elapsed / timeToAppearFromGround);
            col.offset = offset;
            col.size = size;
            elapsed += Time.deltaTime;
            yield return null;
        }
        col.offset = Vector2.zero;
        col.size = new Vector2(1, 1);
        _soldier.GetComponent<MeshRenderer>().enabled = true;
        _expiredCoroutine = StartCoroutine(ExpireSoldier());
        yield return null;
    }

    // Kills the soldier if timeToExpire has been reached.
    private IEnumerator ExpireSoldier()
    {
        yield return new WaitForSeconds(timeToExpire);
        Destroy(_soldier);
    }

    // Useless but I don't want to remove it ...
    /*private Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = _playerInput.CharacterActions.MousePosition.ReadValue<Vector2>();
        if (Camera.main != null)
        {
            pos.z = Mathf.Abs(Camera.main.gameObject.transform.position.z);
            pos = Camera.main.ScreenToWorldPoint(pos);
        }
        pos.z = 0;
        return pos;
    }*/

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        GUIStyle greenStyle = new GUIStyle();
        GUIStyle redStyle = new GUIStyle();
        redStyle.normal.textColor = Color.red;
        greenStyle.normal.textColor = Color.green;

        Handles.Label(transform.position + Vector3.up * 4f, "Pressing : " + (_previewCoroutine != null ? "Yes" : "No"),
            _previewCoroutine != null ? greenStyle : redStyle);
#endif
        Gizmos.color = Color.blue;
        foreach (var hit in raycastHits)
        {
            Gizmos.DrawSphere(hit, 0.2f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_debugPosition, 0.2f);
    }
}
