using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandSoldierBehaviour : MonoBehaviour
{
    public float TimeUntilDeath { get; set; }
    public float TimeToMaxRange { get; set; }
    public float TimeToAppearFromGround { get; set; }

    private float _minRange;
    private float _maxRange;
    public float FinalX { get; set; }

    private SandSoldier _caster;
    private PlayerMovement _playerMovement;

    private Coroutine _previewCoroutine;
    private Coroutine _summonCoroutine;

    private bool _spawned = false;

    public Vector3 StartPosition { get; set; }
    private float _raycastYOffset; // Used to automatically walk over small steps

    public LayerMask BlockingLayer { get; set; }

    private void Update()
    {
        if (!_spawned) return;

        TimeUntilDeath -= Time.deltaTime;

        if (TimeUntilDeath > 0.0f) return;
        
        Destroy(gameObject);
        _caster.SoldiersCount--;
    }

    public void Preview()
    {
        _previewCoroutine = StartCoroutine(PreviewCoroutine());
    }
    
    private IEnumerator PreviewCoroutine()
    {
        Vector3 soldierPosition = StartPosition;

        float elapsed = 0f;
        while (elapsed < TimeToMaxRange)
        {
            elapsed += Time.deltaTime;

            float currentX = Mathf.Lerp(StartPosition.x, FinalX, elapsed / TimeToMaxRange);

            Vector3 raycastSource = new Vector3(currentX, soldierPosition.y + _raycastYOffset, start.z);
            
            // Check for ground beneath current position
            RaycastHit2D groundHit = Physics2D.Raycast(
                raycastSource,
                Vector2.down,
                10f,
                BlockingLayer);
            if (groundHit)
                soldierPosition.y = groundHit.point.y +
                                    GetComponent<BoxCollider2D>().size.y / 2f * transform.localScale.y;
            soldierPosition.x = currentX;
            
            // Check if a wall is in the way
            RaycastHit2D wallHit = Physics2D.Raycast(
                raycastSource,
                _playerMovement.isFacingRight ? Vector2.right : Vector2.left,
                transform.localScale.x / 2f,
                BlockingLayer);
            if (wallHit)
            {
                Summon();
                yield break;
            }

            yield return null;
        }
    }

    public void Summon()
    {
        StopCoroutine(_previewCoroutine);
        _summonCoroutine = StartCoroutine(SummonCoroutine());
    }

    private IEnumerator SummonCoroutine()
    {
        
    }
}
