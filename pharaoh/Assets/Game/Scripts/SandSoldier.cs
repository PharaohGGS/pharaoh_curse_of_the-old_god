using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class SandSoldier : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Prefab for the placement preview.")]
    public GameObject previewIndicator;
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
    [Tooltip("Pick every layer which represent the ground, used to snap the objects to the ground.")]
    public LayerMask groundLayer;

    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput; // Input System
    private Coroutine _previewCoroutine = null; // Stores the preview coroutine
    private GameObject _soldierPreviewInstance; // Stores the reference of the preview object
    private GameObject _soldier; // Stores the reference of the final sand soldier

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
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
        if (_soldier != null) // If there's already a sand soldier, destroy it
            Destroy(_soldier);
        
        // Calculate the start position of the preview from minRange and model rotation.
        Vector3 startPosition = transform.position + new Vector3(minRange, 0, 0) * (playerModel.rotation.eulerAngles.y > 150 ? -1 : 1);
        _soldierPreviewInstance = Instantiate(previewIndicator, startPosition, Quaternion.identity);
        _previewCoroutine = StartCoroutine(PreviewSoldier(startPosition));
    }

    private void SummonSoldier(InputAction.CallbackContext obj)
    {
        // TODO :
        // - Create an empty game object with a collider
        // - Create a particle from each preview particle.
        // - Assign a position on the skinned mesh to each particle.
        // - Increase the collider size along time.
        // - Lerp particles position based on collider time to get max size.

        if (_previewCoroutine == null) return;
        
        StopCoroutine(_previewCoroutine);
        // float yOffset = sandSoldier.transform.localScale.y / 2f -
        //                 previewIndicator.transform.localScale.y / 2f;
        // Vector3 pos = _soldierPreviewInstance.transform.position + new Vector3(0f, yOffset, 0f);
        // _soldier = Instantiate(sandSoldier, pos, Quaternion.identity);
        _soldier = Instantiate(sandSoldier, _soldierPreviewInstance.transform.position, Quaternion.identity);
        Destroy(_soldierPreviewInstance);
        _soldierPreviewInstance = null;
        _previewCoroutine = null;
    }

    private IEnumerator PreviewSoldier(Vector3 startPosition)
    {
        // Ideas :
        // - Pick random points between min and max range to create a bezier curve for the particles to follow.
        
        Vector3 endPosition = startPosition + new Vector3(maxRange - minRange, 0, 0) * (playerModel.rotation.eulerAngles.y > 150 ? -1 : 1);
        float elapsed = 0f;
        while (elapsed < timeToMaxRange)
        {
            Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, elapsed / timeToMaxRange);
            RaycastHit2D hit = Physics2D.Raycast(nextPosition, Vector2.down, 10f, groundLayer);
            if (hit)
                nextPosition = hit.point + new Vector2(0f, _soldierPreviewInstance.transform.localScale.y/2f);
            
            _soldierPreviewInstance.transform.position = nextPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }
        _soldierPreviewInstance.transform.position = endPosition;

        InputAction.CallbackContext obj = new InputAction.CallbackContext();
        SummonSoldier(obj);
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
