using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Gameplay.Component;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GlobalObjectId id;
    public Vector3 SpawnPosition { get; private set; }
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector3 _initialScale;

    private void Awake()
    {
        id = GlobalObjectId.GetGlobalObjectIdSlow(this);

        Transform t = transform;
        _initialPosition = t.position;
        _initialRotation = t.rotation;
        _initialScale = t.localScale;

        if (!LevelManager.Instance.loadEnemyStateOnStart) return;
        EnemyData data = SaveManager.LoadEnemy(id);
        if (data == null) return;
        if (!data.isAlive)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (TryGetComponent(out HealthComponent healthComponent))
            healthComponent.OnHealthUnderZero += Die;
    }
    
    private void OnDisable()
    {
        if (TryGetComponent(out HealthComponent healthComponent))
            healthComponent.OnHealthUnderZero -= Die;
    }

    private void Die(HealthComponent healthComponent)
    {
        SaveManager.SaveEnemy(this);
        Destroy(gameObject);
    }

    public void Reset()
    {
        Transform t = transform;
        t.position = _initialPosition;
        t.rotation = _initialRotation;
        t.localScale = _initialScale;
        if (TryGetComponent(out HealthComponent healthComponent))
            healthComponent.TryUpdateHealth(healthComponent.MaxHealth, HealthOperation.Define, out _);
    }
}
