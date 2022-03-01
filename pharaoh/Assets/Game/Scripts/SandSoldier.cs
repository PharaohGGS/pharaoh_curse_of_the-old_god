using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SandSoldier : MonoBehaviour
{
    public GameObject soldierPreviewModel;

    private PlayerInput _playerInput;
    private Coroutine _previewCoroutine;
    private GameObject _soldierPreviewInstance;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.CharacterActions.Soldier.started += SummonSoldier;
        _playerInput.CharacterActions.Soldier.canceled += SummonSoldier;
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.CharacterActions.Soldier.started -= SummonSoldier;
        _playerInput.CharacterActions.Soldier.canceled -= SummonSoldier;
    }

    private void SummonSoldier(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            _soldierPreviewInstance = Instantiate(soldierPreviewModel, GetMouseWorldPosition(), Quaternion.identity);
            _previewCoroutine = StartCoroutine(PreviewSoldier());
        }
        else if (obj.canceled)
        {
            StopCoroutine(_previewCoroutine);
            Destroy(_soldierPreviewInstance);
            _soldierPreviewInstance = null;
            // TODO: Check if there is a valid spot by ray-casting to floor.
        }
    }

    private IEnumerator PreviewSoldier()
    {
        while (true)
        {
            // TODO: Snap preview position to floor.
            _soldierPreviewInstance.transform.position = GetMouseWorldPosition();
            yield return null;
        }
    }

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
}
