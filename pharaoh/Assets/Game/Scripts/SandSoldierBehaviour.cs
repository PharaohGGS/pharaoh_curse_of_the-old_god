using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
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

    private MeshRenderer _meshRenderer;
    private Collider2D _col;

    private bool _spawned = false;

    public Vector3 StartPosition { get; set; }
    private float _raycastYOffset = 1f; // Used to automatically walk over small steps

    public LayerMask BlockingLayer { get; set; }

    private void Awake()
    {
        _playerMovement = GameManager.Instance.Player.GetComponent<PlayerMovement>();
        _caster = GameManager.Instance.Player.GetComponent<SandSoldier>();
    }

    private void Start()
    {
        if (!TryGetComponent(out _meshRenderer) || !TryGetComponent(out _col))
            throw new Exception();
        _meshRenderer.enabled = false;
        _col.enabled = false;
    }


    private void Update()
    {
        if (!_spawned) return;

        TimeUntilDeath -= Time.deltaTime;

        if (TimeUntilDeath > 0.0f) return;
        
        _caster.SoldiersCount--;
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public void Preview()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            transform.position,
            new Vector2(transform.localScale.x, transform.localScale.y) * 0.9f,
            0f,
            Vector2.zero,
            0f,
            BlockingLayer);
        if (hits.Length > 0)
        {
            transform.position = _caster.transform.position;
            Summon();
            return;
        }
        
        _previewCoroutine = StartCoroutine(PreviewCoroutine());
    }
    
    private IEnumerator PreviewCoroutine()
    {
        Vector3 soldierPosition = StartPosition;

        float elapsed = 0f;
        while (elapsed < TimeToMaxRange)
        {
            elapsed += Time.deltaTime;
            
            // On a 2 positions de Raycast
            // P1: Y initial du joueur
            // P2: Y du soldat + un offset correspondant à sa hauteur d'escalade
            
            // ===== WALL CHECK ===== //
            // WB1: Boxcast de P2 vers le mur
            // WB2 : Boxcast du soldat vers le mur
            // if WB1 et WB2 => On a rencontré un mur trop haut pour être escaladé
                // STOP
            // if WB1 et not WB2 => le soldat passe sous une plateforme
            // if WB2 et not WB1 => le soldat rencontre un mur mais peut l'escalader


            // ===== GROUND CHECK ===== //
            // GB1: Raycast de P1 vers le mur
            // GB2: Raycast de P1 vers le sol
            // if not GB1 and GB2 and le soldat a la place
                // position = GB1.HitPoint
            // if (GB2 and le soldat n'a pas la place) or GB1 => le joueur est en contrebas ou une plateforme en hauteur est détectée mais pas adaptée OU un mur est sur le trajet
                // GB3: Raycast du soldat vers le sol => si la position est dans le sol, le résultat sera la surface supérieur du sol en question
                // position = GB3.HitPoint
                
            // TODO :
            // Fix le problème où la coroutine plante quand on est collé à un mur au départ.
            // Détruire le soldat le plus ancien si la capacité max a été atteinte
            // Faire sortir le soldat du sol
            // Snap le soldat aux bordures (si un bout dépasse dans le vide ou est dans un mur)
            // Ajouter commande de destruction de tous les soldats
                
            float currentX = Mathf.Lerp(StartPosition.x, FinalX, elapsed / TimeToMaxRange);
            float deltaX = Mathf.Abs(soldierPosition.x - currentX);

            Vector2 P1 = new Vector2(currentX, StartPosition.y);
            Vector2 P2 = new Vector2(currentX, soldierPosition.y + _raycastYOffset);
            
            RaycastHit2D WB1 = Physics2D.Raycast(
                P2, // Origin
                _playerMovement.isFacingRight ? Vector2.right : Vector2.left, // Direction
                transform.localScale.x / 2f + deltaX / 2f, // Distance
                BlockingLayer // Layer Mask
                );
            RaycastHit2D WB2 = Physics2D.Raycast(
                soldierPosition, // Origin
                _playerMovement.isFacingRight ? Vector2.right : Vector2.left, // Direction
                transform.localScale.x / 2f + deltaX / 2f, // Distance
                BlockingLayer // Layer Mask
            );

            if (WB1 && WB2)
            {
                Summon();
                yield break;
            }
            
            // Ground check
            RaycastHit2D GB1 = Physics2D.Raycast(
                P1, // Origin
                _playerMovement.isFacingRight ? Vector2.right : Vector2.left, // Direction
                deltaX, // Distance
                BlockingLayer // Layer Mask
            );
            RaycastHit2D GB2 = Physics2D.Raycast(
                P1, // Origin
                Vector2.down, // Direction
                10f, // Distance
                BlockingLayer // Layer Mask
            );
            Vector2 tempPos = new Vector2(
                currentX,
                GB2.point.y + GetComponent<BoxCollider2D>().size.y / 2f * transform.localScale.y);
            RaycastHit2D occupied = Physics2D.BoxCast(
                tempPos, // Origin
                new Vector2(transform.localScale.x, transform.localScale.y) * 0.9f, // Size
                0f, // Angle
                Vector2.zero, // Direction
                0f, // Distance
                BlockingLayer // Layer Mask
                );
            
            if (!GB1 && GB2 && !occupied)
                soldierPosition = new Vector3(
                    currentX,
                    GB2.point.y + GetComponent<BoxCollider2D>().size.y / 2f * transform.localScale.y,
                    StartPosition.z);
            
            if (occupied || GB1)
            {
                RaycastHit2D GB3 = Physics2D.Raycast(
                    soldierPosition, // Origin
                    Vector2.down, // Direction
                    10f, // Distance
                    BlockingLayer // Layer Mask
                );
                soldierPosition = new Vector3(
                    currentX,
                    GB3.point.y + GetComponent<BoxCollider2D>().size.y / 2f * transform.localScale.y,
                    StartPosition.z);
            }

            transform.position = soldierPosition;
            
            yield return null;
        }

        soldierPosition.x = FinalX;
        transform.position = soldierPosition;
        Summon();
        yield return null;
    }

    public void Summon()
    {
        if (_spawned) return;
        _spawned = true;
        if (_previewCoroutine != null)
            StopCoroutine(_previewCoroutine);
        _previewCoroutine = null;
        _summonCoroutine = StartCoroutine(SummonCoroutine());
    }

    private IEnumerator SummonCoroutine()
    {
        // If overlaps with another soldier, destroy it
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            transform.position,
            new Vector2(transform.localScale.x, transform.localScale.y) * 0.9f,
            0f,
            Vector2.zero,
            0f,
            1 << gameObject.layer);
        foreach (var hit in hits)
        {
            Destroy(hit.collider.gameObject);
        }
        
        _meshRenderer.enabled = true;
        _col.enabled = true;
        
        yield return null;
    }
}
