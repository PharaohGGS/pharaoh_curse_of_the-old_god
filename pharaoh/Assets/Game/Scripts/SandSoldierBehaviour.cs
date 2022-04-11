using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Managers;
using UnityEngine;
using UnityEngine.Events;

public class SandSoldierBehaviour : MonoBehaviour
{
    public UnityAction onSummon; // Event to summon the soldier if preview has encountered an obstacle
    [HideInInspector] public Vector2 soldierSize; // Given by SandSoldier script, used to boxcast
    [HideInInspector] public float yOffset = 0.8f;
    [HideInInspector] public float minRange = 1f;
    [HideInInspector] public float maxRange = 6f;
    [HideInInspector] public float timeToMaxRange = 1f;
    [HideInInspector] public LayerMask blockingLayer;
    private PlayerMovement _playerMovement;

    private Vector3 _startPosition; // Stores the position where the preview started
    private Vector3 _estimatedPosition; // Stores the estimated position if soldier has to be summoned at the next frame
    public RaycastHit2D groundHit, wallHit; // Stores raycast values
    private bool _summoned; // Flag to prevent behaviour from changing values if the soldier has been summoned
    private float endX; // Stores x value at max range
    private float elapsed; // Stores elapsed time since Start()


    private void Start()
    {
        if (!CameraManager.Instance.player.TryGetComponent(out _playerMovement))
        {
            Debug.Log("Player from CameraManager has no PlayerMovement");
            Destroy(gameObject);
        }

        // Calculate start position based on minimum range, y offset and player's facing side
        _startPosition = transform.position + new Vector3(minRange, yOffset, 0) * (_playerMovement.IsFacingRight ? 1 : -1);

        // Check if start position is going through an obstacle
        wallHit = Physics2D.Raycast(
            transform.position,
            _playerMovement.IsFacingRight ? Vector2.right : Vector2.left,
            minRange,
            blockingLayer);
        // If it does, summon snapped to the obstacle
        if (wallHit)
        {
            Debug.Log(wallHit.collider.gameObject.name);
            _estimatedPosition.x = wallHit.point.x -
                                   soldierSize.x / 2f *
                                   (_playerMovement.IsFacingRight ? 1 : -1);
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

        _estimatedPosition = _startPosition; // Initialize estimated position
        endX = _startPosition.x + (maxRange - minRange) * (_playerMovement.IsFacingRight ? 1 : -1); // Calculate maximum x
    }

    private void Update()
    {
        if (_summoned) return;

        elapsed += Time.deltaTime;
        // Summon if timeToMaxRange has been reached
        if (elapsed >= timeToMaxRange)
        {
            onSummon?.Invoke();
            _summoned = true;
            return;
        }
        
        // Current frame x value
        float newX = Mathf.Lerp(_startPosition.x, endX, elapsed / timeToMaxRange);

        // Check for ground beneath
        Vector2 raycastPos = new Vector2(newX, _estimatedPosition.y);
        groundHit = Physics2D.Raycast(
            raycastPos,
            Vector2.down,
            10f,
            blockingLayer);
        // Check forward for blocking obstacle
        wallHit = Physics2D.Raycast(
            raycastPos,
            _playerMovement.IsFacingRight ? Vector2.right : Vector2.left,
            soldierSize.x / 2f,
            blockingLayer);

        _estimatedPosition.x = newX; // Update estimated x position with the new x value
        if (groundHit)
            _estimatedPosition.y = groundHit.point.y + soldierSize.y / 2f; // Update estimated y position snapped to ground

        // If there is a wall, snap the soldier to it and summon
        if (wallHit)
        {
            _estimatedPosition.x = wallHit.point.x -
                                   soldierSize.x / 2f *
                                   (_playerMovement.IsFacingRight ? 1 : -1);
            transform.position = _estimatedPosition;
            onSummon?.Invoke();
            _summoned = true;
            return;
        }

        transform.position = _estimatedPosition; // Update the preview position

    }
}
