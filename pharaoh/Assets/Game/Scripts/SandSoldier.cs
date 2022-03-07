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
    
    [Header("References")]
    [Tooltip("Place the player's model reference, used to get player facing direction.")]
    public Transform playerModel;
    
    [Header("Parameters")]
    [Tooltip("Sand soldier minimum range from player.")]
    public float minRange;
    [Tooltip("Sand soldier maximum range from player.")]
    public float maxRange;
    [Tooltip("Pretty explicit, how much time the preview takes to go to the max range.")]
    public float timeToMaxRange;
    public float timeToAppearFromGround;
    [Tooltip("Pick every layer which represent the ground, used to snap the objects to the ground.")]
    public LayerMask groundLayer;

    [Header("VFX")]
    [Tooltip("Soldier position preview VFX")]
    public VisualEffect previewVFX;

    private PlayerInput _playerInput; // Input System
    private Coroutine _previewCoroutine; // Stores the preview coroutine
    private Coroutine _colliderCoroutine;
    private GameObject _soldier; // Stores the reference of the final sand soldier
    private Vector3 _soldierPosition;
    private bool _summoned;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.CharacterActions.Soldier.started += InitiateSummon; // Pressed
        _playerInput.CharacterActions.Soldier.canceled += SummonSoldier; // Released
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.CharacterActions.Soldier.started -= InitiateSummon;
        _playerInput.CharacterActions.Soldier.canceled -= SummonSoldier;
    }

    private void InitiateSummon(InputAction.CallbackContext obj)
    {
        StopAllCoroutines();
        _summoned = false;
        if (_soldier != null) // If there's already a sand soldier, destroy it
            Destroy(_soldier);
        // Calculate the start position of the preview from minRange and model rotation.
        Vector3 startPosition = transform.position + new Vector3(minRange, 0, 0) * (playerModel.rotation.eulerAngles.y > 150 ? -1 : 1);
        //_soldierPreviewInstance = Instantiate(previewIndicator, startPosition, Quaternion.identity);
        _previewCoroutine = StartCoroutine(PreviewSoldier(startPosition));
        previewVFX.SetVector3("KillBoxSize", Vector3.zero);
    }

    private void SummonSoldier(InputAction.CallbackContext obj)
    {
        // TODO :
        // - Create an empty game object with a collider
        // - Create a particle from each preview particle.
        // - Assign a position on the skinned mesh to each particle.
        // - Increase the collider size along time.
        // - Lerp particles position based on collider time to get max size.
        if (_summoned) return;
        _summoned = true;

        //if (_colliderCoroutine != null) StopCoroutine(_colliderCoroutine);
        StopCoroutine(_previewCoroutine);
        previewVFX.SetVector3("KillBoxSize", new Vector3(50, 50, 50));
        previewVFX.Stop();
        // float yOffset = sandSoldier.transform.localScale.y / 2f -
        //                 previewIndicator.transform.localScale.y / 2f;
        // Vector3 pos = _soldierPreviewInstance.transform.position + new Vector3(0f, yOffset, 0f);
        // _soldier = Instantiate(sandSoldier, pos, Quaternion.identity);
        RaycastHit2D groundHit = Physics2D.Raycast(_soldierPosition, Vector2.down, 10f, groundLayer);
        if (groundHit)
            _soldierPosition = groundHit.point + new Vector2(0, sandSoldier.GetComponent<BoxCollider2D>().size.y/2f * sandSoldier.transform.localScale.y);
        _soldier = Instantiate(sandSoldier, _soldierPosition, Quaternion.identity);
        // Vector3 rotation = Vector3.zero;
        // rotation.y = playerModel.rotation.eulerAngles.y > 150 ? -90 : 90;
        // _soldier.transform.GetChild(0).rotation = Quaternion.Euler(rotation);
        _colliderCoroutine = StartCoroutine(MoveSoldierCollider(_soldier.GetComponent<BoxCollider2D>()));
        //Destroy(_soldierPreviewInstance);
        _previewCoroutine = null;
    }

    private IEnumerator PreviewSoldier(Vector3 startPosition)
    {
        // Ideas :
        // - Pick random points between min and max range to create a bezier curve for the particles to follow.
        previewVFX.gameObject.transform.position = transform.position;
        previewVFX.Play();
        Vector3 endPosition = startPosition + new Vector3(maxRange - minRange, 0, 0) * (playerModel.rotation.eulerAngles.y > 150 ? -1 : 1);
        float elapsed = 0f;
        InputAction.CallbackContext obj = new InputAction.CallbackContext();
        while (elapsed < timeToMaxRange)
        {
            Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, elapsed / timeToMaxRange);
            previewVFX.SetVector3("TargetPosition", nextPosition - previewVFX.gameObject.transform.position);
            _soldierPosition = nextPosition;
            RaycastHit2D wallHit = Physics2D.Raycast(nextPosition, endPosition - nextPosition,
                sandSoldier.transform.localScale.x / 2f, groundLayer);
            if (wallHit)
            {
                SummonSoldier(obj);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        previewVFX.SetVector3("TargetPosition", endPosition - transform.position);
        _soldierPosition = endPosition;

        SummonSoldier(obj);
        yield return null;
    }

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
        //_soldier.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        _soldier.GetComponent<MeshRenderer>().enabled = true;
        yield return null;
    }

    // Useless but I don't want to remove it ...
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = _playerInput.CharacterActions.MousePosition.ReadValue<Vector2>();
        if (Camera.main != null)
        {
            pos.z = Mathf.Abs(Camera.main.gameObject.transform.position.z);
            pos = Camera.main.ScreenToWorldPoint(pos);
        }
        pos.z = 0;
        return pos;
    }

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
