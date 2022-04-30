using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Pharaoh.Gameplay.Components.Movement;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using PlayerInput = Pharaoh.Tools.Inputs.PlayerInput;
using AudioManager = Pharaoh.Managers.AudioManager;

public class SandSoldier : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Prefab for the final sand soldier.")]
    public GameObject sandSoldier;
    [Tooltip("Prefab for the soldier preview.")]
    public GameObject soldierPreview;

    [Header("Inputs")]
    public InputReader inputReader;

    [Header("Soldier Parameters")]
    [Tooltip("How much time does it take from button release to fully grown soldier. A low value can create conflicts with other colliders.")]
    public float timeToAppearFromGround = 0.35f;
    [Tooltip("Soldier's life expectancy.")]
    public float timeToExpire = 10f;
    [Tooltip("Time left to other soldiers if a new one is summoned")]
    public float maximumTimeIfMultipleSoldiers = 3f;
    [Tooltip("Maximum number of soldiers")]
    public int maxSoldiers = 2;
    [Tooltip("Soldier width and height (Ref: x:1.5, y:2.1")]
    public Vector2 soldierSize = new Vector2(1.5f, 2.1f);
    [Tooltip("Particles force to reach soldier shape")]
    public float particleForce = 10f;
    [Tooltip("Time for the soldier to collapse")]
    public float timeToDie = 1f;

    [Header("Preview parameters")]
    [Tooltip("Small Y offset to avoid the preview getting blocked by small steps")]
    public float yOffset = 0.8f;
    [Tooltip("Minimum range value for the soldier preview")]
    public float minRange = 1f;
    [Tooltip("Maximum range value for the soldier preview")]
    public float maxRange = 6f;
    [Tooltip("Time for the preview to reach maximum range")]
    public float timeToMaxRange = 1f;
    
    [Header("Layers")]
    [Tooltip("Pick every layer which represent the ground / walls, used to snap the objects to the ground.")]
    public LayerMask blockingLayer;
    [Tooltip("Sand soldier layer, used to prevent overlapping")]
    public LayerMask soldierLayer;
    [Tooltip("Moving Block layer, used to avoid the block getting stuck")]
    public LayerMask movingBlockLayer;
    [Tooltip("Player layer, used to avoid the player getting stuck")]
    public LayerMask playerLayer;

    [Header("Spawning parameters")]
    public Vector2 startColliderOffset = new Vector2(0.1f, 0f);
    public Vector2 endColliderOffset = new Vector2(0.1f, 1.05f);
    public Vector2 startColliderSize = new Vector2(1.65f, 0f);
    public Vector2 endColliderSize = new Vector2(1.65f, 2.15f);

    [Header("Tweaks")]
    [Range(0f, 1f)] public float blockPlusSoldierThreshold = 0.7f;

    [Header("Animations")]
    public Animator animator;

    private bool _longPress; // Bool to check if the input has been held or not
    private PlayerMovement _playerMovement;

    private List<GameObject> _soldiers; // Stores all sand soldiers GameObject
    private Dictionary<int, float> _soldiersLifetime; // Stores SandSoldierInstanceID => Time left before disappearing
    private Dictionary<int, Coroutine> _soldiersExpireCoroutine; // Stores coroutines that counts second before soldier disappears
    private SandSoldierBehaviour _soldierBehaviour; // Stores current soldier behaviour

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _soldiers = new List<GameObject>();
        _soldiersLifetime = new Dictionary<int, float>();
        _soldiersExpireCoroutine = new Dictionary<int, Coroutine>();
    }

    private void OnEnable()
    {
        inputReader.sandSoldierStartedEvent += ResetFlags;
        inputReader.sandSoldierPerformedEvent += InitiateSummon;
        inputReader.sandSoldierCanceledEvent += SummonSoldier;
        inputReader.killAllSoldiersStartedEvent += KillAllSoldiers;
    }

    private void OnDisable()
    {
        inputReader.sandSoldierStartedEvent -= ResetFlags;
        inputReader.sandSoldierPerformedEvent -= InitiateSummon;
        inputReader.sandSoldierCanceledEvent -= SummonSoldier;
        inputReader.killAllSoldiersStartedEvent -= KillAllSoldiers;
    }

    // Called at InputActionPhase.Started
    // Always called when input is pressed
    //      Reset every flag
    //      Disable player actions
    private void ResetFlags()
    {
        _longPress = false;
        if (_playerMovement.skills.HasSandSoldier)
        {
            DisableActions();
        }
    }
    
    // Called at InputActionPhase.Performed
    // Called when the input has been held long enough
    //      Instantiate Soldier Preview which has a SandSoldierBehaviour on it
    private void InitiateSummon()
    {
        // checks if player has skill unlocked
        if (!_playerMovement.skills.HasSandSoldier) return;
        if (_playerMovement.IsPullingBlock) return;
        
        animator.SetTrigger("Summon");

        _longPress = true;

        Vector3 spawnPos = transform.position;
        spawnPos.y += soldierSize.y / 2f;
        GameObject go = Instantiate(soldierPreview, spawnPos, Quaternion.identity);
        if (!go.TryGetComponent(out _soldierBehaviour))
        {
            Debug.Log("No Sand Soldier Behaviour");
            Destroy(go);
            return;
        }

        // Give preview values to the behaviour
        _soldierBehaviour.yOffset = yOffset;
        _soldierBehaviour.minRange = minRange;
        _soldierBehaviour.maxRange = maxRange;
        _soldierBehaviour.timeToMaxRange = timeToMaxRange;
        _soldierBehaviour.blockingLayer = blockingLayer;
        _soldierBehaviour.soldierSize = soldierSize;
        _soldierBehaviour.onSummon += SummonSoldier;
    }

    // Called at InputActionPhase.Canceled
    // Always called when the player release the input
    private void SummonSoldier()
    {
        EnableActions(); // Re-enable player inputs

        // checks if player has skill unlocked
        if (!_playerMovement.skills.HasSandSoldier) return;
        if (_playerMovement.IsPullingBlock) return;
        
        // If it's a simple press, summon under player
        if (!_longPress)
        {
            StandSummon();
            return;
        }

        if (!_soldierBehaviour.groundHit) // If last ground hit was failed => Soldier is above a gap so don't summon
        {
            Destroy(_soldierBehaviour.gameObject);
            return;
        }

        if (_soldierBehaviour == null) return; // If behaviour has been destroyed for whatever reason, stop

        Vector3 position = _soldierBehaviour.transform.position; // Get final position (x,y)
        position.z = transform.position.z; // Make sure z is correct

        // Check if the position overlaps with other soldiers.
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            position,
            soldierSize * 0.9f,
            0f,
            Vector2.zero,
            0f,
            soldierLayer);
        Destroy(_soldierBehaviour.gameObject);
        // Kill every overlapping soldiers detected above
        foreach (var hit in hits)
        {
            StartCoroutine(KillSoldier(hit.collider.gameObject));
        }

        HandleSoldiers(); // Remove extra soldiers and reduce timer of the others

        GameObject soldier = Instantiate(sandSoldier, position, Quaternion.identity); // Instantiate the true soldier
        
        _soldiers.Add(soldier); // Store the soldier to keep a track
        _soldiersLifetime.Add(soldier.GetInstanceID(), timeToExpire); // Add soldier timer to the list
        StartCoroutine(SoldierSpawning(soldier)); // Start spawning coroutine
    }

    // Called if the input hasn't been held long enough
    //      Summon the soldier under the player
    private void StandSummon()
    {
        Vector3 position = transform.position;
        
        // Raycast to ground from player
        RaycastHit2D groundHit = Physics2D.Raycast(
            position,
            Vector2.down,
            10f,
            blockingLayer);

        // Snap the soldier y to the ground
        position.y = groundHit.point.y + soldierSize.y / 2f;
        
        // Check if soldier has enough space to spawn
        // (Boxcasting may also detect the ground underneath so better check left and right
        RaycastHit2D leftWallHit = Physics2D.Raycast(
            position,
            Vector2.left,
            soldierSize.x / 2f,
            blockingLayer);
        RaycastHit2D rightWallHit = Physics2D.Raycast(
            position,
            Vector2.right,
            soldierSize.x / 2f,
            blockingLayer);

        if (leftWallHit && rightWallHit) return; // Doesn't summon if not enough space

        // Snap the soldier to a wall if one has been detected
        if (leftWallHit)
            position.x = leftWallHit.point.x + soldierSize.x / 2f;
        else if (rightWallHit)
            position.x = rightWallHit.point.x - soldierSize.x / 2f;
        
        // Check if the position overlaps with other soldiers.
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            position,
            soldierSize * 0.9f,
            0f,
            Vector2.zero,
            0f,
            soldierLayer);
        // Kill every overlapping soldiers detected above
        foreach (var hit in hits)
        {
            StartCoroutine(KillSoldier(hit.collider.gameObject));
        }
        
        HandleSoldiers(); // Remove extra soldiers and reduce timer of the others

        GameObject soldier = Instantiate(sandSoldier, position, Quaternion.identity); // Instantiate the true soldier
        _soldiers.Add(soldier); // Store the soldier to keep a track
        _soldiersLifetime.Add(soldier.GetInstanceID(), timeToExpire); // Add soldier timer to the list
        StartCoroutine(SoldierSpawning(soldier)); // Start spawning coroutine
    }

    // Remove extra soldiers and reduce timer of the others
    private void HandleSoldiers()
    {
        if (_soldiers.Count >= maxSoldiers) // Kill older soldier if max has been reached
            StartCoroutine(KillSoldier(_soldiers[0]));

        // Reduce all soldiers' timer
        List<int> ids = new List<int>(_soldiersLifetime.Keys);
        foreach (int id in ids)
        {
            if (_soldiersLifetime[id] > maximumTimeIfMultipleSoldiers)
                _soldiersLifetime[id] = maximumTimeIfMultipleSoldiers;
        }
    }

    // Lerp the soldier collider size and offset from 0 to full size
    // Used to lift object
    private IEnumerator SoldierSpawning(GameObject soldier)
    {
        if (!soldier.TryGetComponent(out BoxCollider2D col)) yield break;
        if (!soldier.transform.GetChild(0).TryGetComponent(out MeshRenderer meshRenderer)) yield break;
        if (!soldier.TryGetComponent(out VisualEffect vfx)) yield break;

        meshRenderer.enabled = false; // Hide the mesh
        vfx.SetFloat("Force", particleForce);
        vfx.SetFloat("RotateAngle", _playerMovement.IsFacingRight ? -90 : 90);
        vfx.SetFloat("TimeToAppear", timeToAppearFromGround);
        vfx.SendEvent("Spawn");

        // Rotate the soldier based on player's facing side
        Vector3 scale = soldier.transform.GetChild(0).localScale;
        scale.z = _playerMovement.IsFacingRight ? 1 : -1;
        soldier.transform.GetChild(0).localScale = scale;
        
        // Check if soldier has enough height space to spawn
        RaycastHit2D roomCheck = Physics2D.BoxCast(
            soldier.transform.position,
            col.bounds.size * 0.9f,
            0f,
            Vector2.up,
            0f,
            blockingLayer);
        if (roomCheck)
        {
            Destroy(soldier); // If not, destroy it
            yield break;
        }
        
        // If there's a moving block, check if there is space for soldier's height + block's height
        RaycastHit2D movingBlockCheck = Physics2D.BoxCast(
            soldier.transform.position,
            soldierSize * 0.9f,
            0f,
            Vector2.up,
            0f,
            movingBlockLayer);
        if (movingBlockCheck)
        {
            Vector2 size = movingBlockCheck.collider.bounds.size;
            Vector2 checkSize = new Vector2(size.x, soldierSize.y + size.y);
            Vector3 pos = soldier.transform.position;
            pos.y += -(soldierSize.y / 2f) + (checkSize.y / 2f);
            RaycastHit2D movingBlockRoomCheck = Physics2D.BoxCast(
                pos,
                checkSize * blockPlusSoldierThreshold,
                0f,
                Vector2.up,
                0f,
                blockingLayer);
            if (movingBlockRoomCheck)
            {
                Destroy(soldier);
                yield break;
            }
        }
        
        RaycastHit2D playerCheck = Physics2D.BoxCast(
            soldier.transform.position,
            soldierSize * 0.9f,
            0f,
            Vector2.up,
            0f,
            playerLayer);
        if (playerCheck)
        {
            Vector2 size = playerCheck.collider.bounds.size;
            Vector2 checkSize = new Vector2(size.x, soldierSize.y + size.y);
            Vector3 pos = soldier.transform.position;
            pos.y += -(soldierSize.y / 2f) + (checkSize.y / 2f);
            RaycastHit2D playerRoomCheck = Physics2D.BoxCast(
                pos,
                checkSize * blockPlusSoldierThreshold,
                0f,
                Vector2.up,
                0f,
                blockingLayer);
            if (playerRoomCheck)
            {
                Destroy(soldier);
                yield break;
            }
        }
        AudioManager.Instance?.Play("SandSoldierOn");

        // Lerp the collider size from ground to full size
        float elapsed = 0f;
        col.offset = startColliderOffset;
        col.size = startColliderSize;
        while (elapsed < timeToAppearFromGround)
        {
            Vector2 offset = col.offset;
            Vector2 size = col.size;
            offset.y = Mathf.Lerp(startColliderOffset.y, endColliderOffset.y, elapsed / timeToAppearFromGround);
            size.y = Mathf.Lerp(startColliderSize.y, endColliderSize.y, elapsed / timeToAppearFromGround);
            col.offset = offset;
            col.size = size;
            elapsed += Time.deltaTime;
            yield return null;
        }
        col.offset = endColliderOffset;
        col.size = endColliderSize;
        meshRenderer.enabled = true; // Show the mesh
        _soldiersExpireCoroutine.Add(soldier.GetInstanceID(), StartCoroutine(SoldierExpiration(soldier))); // Start the timer
        yield return null;
    }
    
    // Counts soldiers' time left before destruction
    private IEnumerator SoldierExpiration(GameObject soldier)
    {
        int id = soldier.GetInstanceID();
        if (!_soldiersLifetime.ContainsKey(id)) yield break;
        
        while (_soldiersLifetime[id] > 0f)
        {
            _soldiersLifetime[id] -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(KillSoldier(soldier));
        yield return null;
    }

    // Destroy a soldier and remove any data about it
    private IEnumerator KillSoldier(GameObject soldier)
    {
        if (soldier == null) yield break;

        int id = soldier.GetInstanceID();
        StopCoroutine(_soldiersExpireCoroutine[id]);
        _soldiers.Remove(soldier);
        _soldiersLifetime.Remove(id);
        _soldiersExpireCoroutine.Remove(id);

        if (soldier.transform.GetChild(0).TryGetComponent(out MeshRenderer meshRenderer))
            meshRenderer.enabled = false;
        if (soldier.TryGetComponent(out Collider2D col))
            col.enabled = false;
        if (!soldier.TryGetComponent(out VisualEffect vfx))
            Debug.Log("No Vfx on soldier");
        vfx.SendEvent("Kill");
        AudioManager.Instance?.Play("SandSoldierOff");
        yield return new WaitForSeconds(timeToDie);
        Destroy(soldier);
    }
    
    // Destroy all soldiers
    private void KillAllSoldiers()
    {
        List<GameObject> soldiers = new List<GameObject>(_soldiers);
        foreach (GameObject soldier in soldiers)
        {
            StartCoroutine(KillSoldier(soldier));
        }
        _soldiers.Clear();
        _soldiersLifetime.Clear();
    }
    
    // Prevent below actions from being executed
    private void DisableActions()
    {
        inputReader.DisableAttack();
        if(_playerMovement.IsGrounded)
        {
            inputReader.DisableMove();
        }
        inputReader.DisableDash();
        inputReader.DisableJump();
        inputReader.DisableHookGrapple();
        inputReader.DisableHookInteract();
    }
    
    // Allow below actions to be executed
    private void EnableActions()
    {
        inputReader.EnableAttack();
        inputReader.EnableMove();
        inputReader.EnableDash();
        inputReader.EnableJump();
        inputReader.EnableHookGrapple();
        inputReader.EnableHookInteract();
    }
}
