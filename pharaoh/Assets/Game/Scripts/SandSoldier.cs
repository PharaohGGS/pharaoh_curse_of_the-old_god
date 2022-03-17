using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public int maxSoldiers = 3;
    public float minCooldown = 3.0f;
    
    [Tooltip("Pretty explicit, how much time the preview takes to go to the max range.")]
    public float timeToMaxRange = 1f;
    [Tooltip("How much time does it take from button release to fully grown soldier. A low value can create conflicts with other colliders.")]
    public float timeToAppearFromGround = 0.35f;
    [Tooltip("Soldier's life expectancy.")]
    public float timeToExpire = 10f;
    [Tooltip("Pick every layer which represent the ground, used to snap the objects to the ground.")]
    public LayerMask groundLayer;

    public int SoldiersCount { get; set; }
    public Vector3 StartPosition { get; set; }
    public Vector3 FinalX { get; set; }

    private Dictionary<GameObject, float> _lifeExpectancies;

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

    private SandSoldierBehaviour _currentSandSoldier;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerMovement = GetComponent<PlayerMovement>();
        _lifeExpectancies = new Dictionary<GameObject, float>();
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
        if (_playerMovement.IsHookedToBlock) return;
        
        Vector3 startPosition = transform.position + new Vector3(minRange, 0, 0) * (_playerMovement.isFacingRight ? 1 : -1);
        GameObject go = Instantiate(sandSoldier, startPosition, Quaternion.identity);

        if (!go.TryGetComponent(out SandSoldierBehaviour behaviour)) return;

        _currentSandSoldier = behaviour;
        _currentSandSoldier.StartPosition = startPosition;
        _currentSandSoldier.TimeUntilDeath = timeToExpire;
        _currentSandSoldier.TimeToMaxRange = timeToMaxRange;
        _currentSandSoldier.TimeToAppearFromGround = timeToAppearFromGround;
        _currentSandSoldier.BlockingLayer = groundLayer;
        _currentSandSoldier.FinalX = startPosition.x + (maxRange - minRange) * (_playerMovement.isFacingRight ? 1 : -1);

        _currentSandSoldier.Preview();
    }

    // Called on button release or on other specific conditions
    // Instantiates the soldier and starts the growing collider coroutine
    private void SummonSoldier(InputAction.CallbackContext obj = new())
    {
        _currentSandSoldier.Summon();
        // if (_playerMovement.IsHookedToBlock) return;
        //
        // if (_summoned) return;
        // _summoned = true;
        //
        // if (_previewCoroutine != null)
        //     StopCoroutine(_previewCoroutine);
        // _previewCoroutine = null;
        //
        // // previewVFX.SetVector3("KillBoxSize", new Vector3(50, 50, 50));
        // // previewVFX.Stop();
        // Destroy(_soldierPreviewInstance);
        //
        // if (!_groundHit) return;
        // _soldierPosition.z = transform.position.z;
        // GameObject soldier = Instantiate(sandSoldier, _soldierPosition, Quaternion.identity);
        // _lifeExpectancies.Add(soldier, timeToExpire);
        //
        // foreach (var key in _lifeExpectancies.Keys.ToList())
        // {
        //     if (_lifeExpectancies[key] > minCooldown)
        //         _lifeExpectancies[key] = minCooldown;
        // }
        //
        // if (_lifeExpectancies.Count > maxSoldiers)
        // {
        //     Destroy(_lifeExpectancies.First().Key);
        //     _lifeExpectancies.Remove(_lifeExpectancies.First().Key);
        // }
        //
        // _colliderCoroutine = StartCoroutine(MoveSoldierCollider(soldier));
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
    private IEnumerator MoveSoldierCollider(GameObject soldier)
    {
        float elapsed = 0f;
        BoxCollider2D col = soldier.GetComponent<BoxCollider2D>();
        col.offset = new Vector2(0, -0.5f);
        col.size = new Vector2(1, 0);
        soldier.GetComponent<MeshRenderer>().enabled = false;
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
        soldier.GetComponent<MeshRenderer>().enabled = true;
        //_expiredCoroutine = StartCoroutine(ExpireSoldier());
        yield return null;
    }

    private void Update()
    {
        List<GameObject> goToRemove = new List<GameObject>();
        foreach (var key in _lifeExpectancies.Keys.ToList())
        {
            _lifeExpectancies[key] -= Time.deltaTime;
            
            if (!(_lifeExpectancies[key] < 0.0f)) continue;
            
            goToRemove.Add(key);
        }

        foreach (var go in goToRemove)
        {
            _lifeExpectancies.Remove(go);
            Destroy(go);
        }
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
    }
}
