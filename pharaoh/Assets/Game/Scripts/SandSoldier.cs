using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Pharaoh.Gameplay.Components.Movement;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;

public class SandSoldier : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Prefab for the final sand soldier.")]
    public GameObject sandSoldier;
    [Tooltip("Prefab for the soldier preview.")]
    public GameObject soldierPreview;

    [Header("Inputs")]
    public InputReader inputReader;
    
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
    public float maximumTimeIfMultipleSoldiers = 3f;
    public int maxSoldiers = 2;
    public Vector2 soldierSize;
    [Tooltip("Pick every layer which represent the ground, used to snap the objects to the ground.")]
    public LayerMask groundLayer;
    public LayerMask soldierLayer;

    // [Header("VFX")]
    // [Tooltip("Soldier position preview VFX")]
    // public VisualEffect previewVFX;

    private PlayerInput _playerInput; // Input System
    private Coroutine _previewCoroutine; // Stores the preview coroutine
    private Coroutine _colliderCoroutine; // Useless to store this coroutine for now
    private Coroutine _expiredCoroutine; // Coroutine to destroy the soldier when its time.
    private Vector3 _soldierPosition; // used to store the soldier position during the preview
    private bool _longPress; // boolean to check whether the soldier has already been summoned (to avoid conflicts)
    private RaycastHit2D _groundHit; // stores whether the current position is over a proper ground
    private PlayerMovement _playerMovement;

    private List<GameObject> _soldiers;
    private Dictionary<int, float> _soldiersLifetime;
    private Dictionary<int, Coroutine> _soldiersExpireCoroutine;
    private SandSoldierBehaviour _soldierPreview;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerMovement = GetComponent<PlayerMovement>();
        _soldiers = new List<GameObject>();
        _soldiersLifetime = new Dictionary<int, float>();
        _soldiersExpireCoroutine = new Dictionary<int, Coroutine>();
    }

    private void OnEnable()
    {
        inputReader.sandSoldierPerformedEvent += InitiateSummon;
        inputReader.sandSoldierCanceledEvent += SummonSoldier;
        inputReader.killAllSoldiersStartedEvent += KillAllSoldiers;
    }

    private void OnDisable()
    {
        inputReader.sandSoldierPerformedEvent -= InitiateSummon;
        inputReader.sandSoldierCanceledEvent -= SummonSoldier;
        inputReader.killAllSoldiersStartedEvent -= KillAllSoldiers;
    }
    
    // Called on button press
    // Cancel / Delete previous summons and start the preview
    private void InitiateSummon()
    {
        if (_playerMovement.IsPullingBlock) return;
        
        _longPress = true;
        
        if (_expiredCoroutine != null)
            StopCoroutine(_expiredCoroutine);

        GameObject go = Instantiate(soldierPreview, transform.position, Quaternion.identity);
        if (!go.TryGetComponent(out _soldierPreview))
        {
            Debug.Log("No Sand Soldier Behaviour");
            Destroy(go);
            return;
        }

        _soldierPreview.onSummon += SummonSoldier;
        // previewVFX.SetVector3("KillBoxSize", Vector3.zero);
    }

    // Called on button release or on other specific conditions
    // Instantiates the soldier and starts the growing collider coroutine
    private void SummonSoldier()
    {
        if (_playerMovement.IsPullingBlock) return;

        if (!_longPress)
        {
            StandSummon();
            return;
        }
        
        _longPress = false;

        if (!_soldierPreview.groundHit) return;

        if (_soldierPreview == null) return;

        Vector3 position = _soldierPreview.transform.position;
        position.z = transform.position.z;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            position,
            soldierSize * 0.9f,
            0f,
            Vector2.zero,
            0f,
            soldierLayer);
        Destroy(_soldierPreview.gameObject);
        foreach (var hit in hits)
        {
            KillSoldier(hit.collider.gameObject);
        }
        
        HandleSoldiers();

        GameObject soldier = Instantiate(sandSoldier, position, Quaternion.identity);
        _soldiers.Add(soldier);
        _soldiersLifetime.Add(soldier.GetInstanceID(), timeToExpire);
        _colliderCoroutine = StartCoroutine(MoveSoldierCollider(soldier));
    }

    private void StandSummon()
    {
        Vector3 position = transform.position;
        
        RaycastHit2D groundHit = Physics2D.Raycast(
            position,
            Vector2.down,
            10f,
            groundLayer);

        position.y = groundHit.point.y + soldierSize.y / 2f;
        
        RaycastHit2D leftWallHit = Physics2D.Raycast(
            position,
            Vector2.left,
            soldierSize.x / 2f,
            groundLayer);
        RaycastHit2D rightWallHit = Physics2D.Raycast(
            position,
            Vector2.right,
            soldierSize.x / 2f,
            groundLayer);

        if (leftWallHit && rightWallHit) return;

        if (leftWallHit)
            position.x = leftWallHit.point.x + soldierSize.x / 2f;
        else if (rightWallHit)
            position.x = rightWallHit.point.x - soldierSize.x / 2f;
        
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            position,
            soldierSize * 0.9f,
            0f,
            Vector2.zero,
            0f,
            soldierLayer);
        foreach (var hit in hits)
        {
            KillSoldier(hit.collider.gameObject);
        }
        
        HandleSoldiers();
        
        GameObject soldier = Instantiate(sandSoldier, position, Quaternion.identity);
        _soldiers.Add(soldier);
        _soldiersLifetime.Add(soldier.GetInstanceID(), timeToExpire);
        _colliderCoroutine = StartCoroutine(MoveSoldierCollider(soldier));
    }

    private void HandleSoldiers()
    {
        if (_soldiers.Count >= maxSoldiers)
            KillSoldier(_soldiers[0]);

        List<int> ids = new List<int>(_soldiersLifetime.Keys);
        foreach (int id in ids)
        {
            if (_soldiersLifetime[id] > maximumTimeIfMultipleSoldiers)
                _soldiersLifetime[id] = maximumTimeIfMultipleSoldiers;
        }
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
            _soldierPreview.transform.position = _soldierPosition;

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
        if (!soldier.TryGetComponent(out MeshRenderer meshRenderer) ||
            !soldier.TryGetComponent(out BoxCollider2D col))
        {
            yield break;
        }
        
        float elapsed = 0f;
        col.offset = new Vector2(0, -0.5f);
        col.size = new Vector2(1, 0);
        meshRenderer.enabled = false;
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
        meshRenderer.enabled = true;
        _soldiersExpireCoroutine.Add(soldier.GetInstanceID(), StartCoroutine(SoldierExpiration(soldier)));
        yield return null;
    }
    
    private IEnumerator SoldierExpiration(GameObject soldier)
    {
        int id = soldier.GetInstanceID();
        while (_soldiersLifetime[id] > 0f)
        {
            _soldiersLifetime[id] -= Time.deltaTime;
            yield return null;
        }
        KillSoldier(soldier);
        yield return null;
    }

    private void KillSoldier(GameObject soldier)
    {
        if (soldier == null) return;
        
        int id = soldier.GetInstanceID();
        StopCoroutine(_soldiersExpireCoroutine[id]);
        _soldiers.Remove(soldier);
        _soldiersLifetime.Remove(id);
        _soldiersExpireCoroutine.Remove(id);
        Destroy(soldier);
    }
    
    private void KillAllSoldiers()
    {
        foreach (GameObject soldier in _soldiers)
        {
            Destroy(soldier);
        }
        _soldiers.Clear();
        _soldiersLifetime.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle greenStyle = new GUIStyle();
        GUIStyle redStyle = new GUIStyle();
        redStyle.normal.textColor = Color.red;
        greenStyle.normal.textColor = Color.green;

        Handles.Label(transform.position + Vector3.up * 4f, "Pressing : " + (_previewCoroutine != null ? "Yes" : "No"),
            _previewCoroutine != null ? greenStyle : redStyle);
    }
#endif
}
