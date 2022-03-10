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

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.CharacterActions.SandSoldier.started += InitiateSummon; // Pressed
        _playerInput.CharacterActions.SandSoldier.canceled += SummonSoldier; // Released
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.CharacterActions.SandSoldier.started -= InitiateSummon;
        _playerInput.CharacterActions.SandSoldier.canceled -= SummonSoldier;
    }

    // Called on button press
    // Cancel / Delete previous summons and start the preview
    private void InitiateSummon(InputAction.CallbackContext obj)
    {
        // TODO
        // Disable player movement
        
        StopAllCoroutines();
        _summoned = false;
        if (_soldier != null) // If there's already a sand soldier, destroy it
            Destroy(_soldier);
        if (_expiredCoroutine != null)
            StopCoroutine(_expiredCoroutine);
        
        // Calculate the start position of the preview from minRange and model rotation.
        Vector3 startPosition = transform.position + new Vector3(minRange, 0, 0) * (playerModel.rotation.eulerAngles.y > 150 ? -1 : 1);
        _soldierPreviewInstance = Instantiate(soldierPreview, startPosition, Quaternion.identity);
        _previewCoroutine = StartCoroutine(PreviewSoldier(startPosition));
        // previewVFX.SetVector3("KillBoxSize", Vector3.zero);
    }

    // Called on button release or on other specific conditions
    // Instantiates the soldier and starts the growing collider coroutine
    private void SummonSoldier(InputAction.CallbackContext obj = new())
    {
        // TODO
        // Re-enable player movement
        
        if (_summoned) return;
        _summoned = true;

        StopCoroutine(_previewCoroutine);
        _previewCoroutine = null;
        
        // previewVFX.SetVector3("KillBoxSize", new Vector3(50, 50, 50));
        // previewVFX.Stop();
        Destroy(_soldierPreviewInstance);
        
        if (!_groundHit) return;
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

        float endX = startPosition.x + (maxRange - minRange) * (playerModel.rotation.eulerAngles.y > 150 ? -1 : 1);
        
        float playerSize = GetComponent<Collider2D>().bounds.size.y;
        
        float elapsed = 0f;
        while (elapsed < timeToMaxRange)
        {
            elapsed += Time.deltaTime;
            
            float newX = Mathf.Lerp(startPosition.x, endX, elapsed / timeToMaxRange);

            Vector3 raycastPos = new Vector3(newX, startPosition.y + playerSize / 2f, startPosition.z);

            _groundHit = Physics2D.Raycast(raycastPos, Vector2.down, 10f, groundLayer);
            RaycastHit2D wallHit = Physics2D.Raycast(
                raycastPos,
                playerModel.rotation.eulerAngles.y > 150 ? Vector2.left : Vector2.right,
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
        GUIStyle greenStyle = new GUIStyle();
        GUIStyle redStyle = new GUIStyle();
        redStyle.normal.textColor = Color.red;
        greenStyle.normal.textColor = Color.green;

        Handles.Label(transform.position + Vector3.up * 4f, "Pressing : " + (_previewCoroutine != null ? "Yes" : "No"),
            _previewCoroutine != null ? greenStyle : redStyle);
    }
}
