using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineSwitcher : MonoBehaviour
{
    
    [SerializeField]
    private InputAction action;

    private Animator _animator;
    private bool _baseCamera;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        action.performed += _ => SwitchState();
    }

    private void SwitchState()
    {
        _animator.Play(_baseCamera ? "VFX" : "Base");
        _baseCamera = !_baseCamera;
    }
}
