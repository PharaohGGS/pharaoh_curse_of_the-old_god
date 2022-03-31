using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Managers;
using UnityEngine;
using UnityEngine.Events;

public class SandSoldierBehaviour : MonoBehaviour
{
    public float yOffset = 0.8f;
    public float minRange = 1f;
    public float maxRange = 6f;
    public float timeToMaxRange = 1f;
    public Vector2 soldierSize;
    public LayerMask blockingLayer;
    
    public UnityAction onSummon;

    private PlayerMovement _playerMovement;

    private Vector3 _startPosition;
    private Vector3 _estimatedPosition;
    public RaycastHit2D groundHit, wallHit;
    private bool _summoned;
    private float endX, elapsed;


    private void Start()
    {
        if (!CameraManager.Instance.player.TryGetComponent(out _playerMovement))
        {
            Debug.Log("Player from CameraManager has no PlayerMovement");
            Destroy(gameObject);
        }

        _startPosition = transform.position + new Vector3(minRange, yOffset, 0) * (_playerMovement.isFacingRight ? 1 : -1);

        // First check if startPosition is in an obstacle
        wallHit = Physics2D.Raycast(
            transform.position,
            _playerMovement.isFacingRight ? Vector2.right : Vector2.left,
            minRange,
            blockingLayer);
        if (wallHit)
        {
            _estimatedPosition.x = wallHit.point.x -
                                   soldierSize.x / 2f *
                                   (_playerMovement.isFacingRight ? 1 : -1);
            groundHit = Physics2D.Raycast(
                new Vector2(_estimatedPosition.x, wallHit.point.y),
                Vector2.down,
                10f,
                blockingLayer);
            _estimatedPosition.y = groundHit.point.y +
                                   soldierSize.y / 2f;
            transform.position = _estimatedPosition;
            onSummon?.Invoke();
            _summoned = true;
            return;
        }

        _estimatedPosition = _startPosition;
        endX = _startPosition.x + (maxRange - minRange) * (_playerMovement.isFacingRight ? 1 : -1);
    }

    private void Update()
    {
        if (_summoned) return;

        elapsed += Time.deltaTime;
        if (elapsed >= timeToMaxRange)
        {
            onSummon?.Invoke();
            _summoned = true;
            return;
        }
        
        float newX = Mathf.Lerp(_startPosition.x, endX, elapsed / timeToMaxRange);

        Vector2 raycastPos = new Vector2(newX, _estimatedPosition.y);
        groundHit = Physics2D.Raycast(
            raycastPos,
            Vector2.down,
            10f,
            blockingLayer);
        wallHit = Physics2D.Raycast(
            raycastPos,
            _playerMovement.isFacingRight ? Vector2.right : Vector2.left,
            soldierSize.x / 2f,
            blockingLayer);

        _estimatedPosition.x = newX;
        if (groundHit)
            _estimatedPosition.y = groundHit.point.y + soldierSize.y / 2f;

        if (wallHit)
        {
            _estimatedPosition.x = wallHit.point.x -
                                   soldierSize.x / 2f *
                                   (_playerMovement.isFacingRight ? 1 : -1);
            transform.position = _estimatedPosition;
            onSummon?.Invoke();
            _summoned = true;
            return;
        }

        transform.position = _estimatedPosition;

    }
}
