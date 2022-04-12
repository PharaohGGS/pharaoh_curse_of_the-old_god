using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{

    private Material _material;
    private float _timer;

    private void Start()
    {
        if (TryGetComponent(out Renderer renderer))
        {
            _material = renderer.material;
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_material)
        {
            _material.SetFloat("_DissolvedAmount", 1.0f - _timer % 1.0f);
        }
    }
}
