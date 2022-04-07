using System;
using PlayerMovement = Pharaoh.Gameplay.Components.Movement.PlayerMovement;
using System.Collections;
using Pharaoh.Tools.Debug;
using UnityEngine;

[Serializable]
public struct Sockets
{
    public Transform hand;
    public Transform back;
}

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{

    private PlayerMovement _playerMovement;
    private short _combatPhase = 0;
    private bool _sheathed = true;
    private Coroutine _sheatheCoroutine;

    [Header("Components")]

    public InputReader inputReader;
    public Animator animator;
    public AnimationEventsReceiver animationEventsReceiver;

    [Header("Sockets")]
    
    public Transform rightSword;
    public Sockets rightSocket;
    private Collider2D _rightCollider;

    public Transform leftSword;
    public Sockets leftSocket;
    private Collider2D _leftCollider;
    
    [Header("Variables")]

    public float timeBeforeSheathing = 5f;


    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        
        if (leftSword?.TryGetComponent(out _leftCollider) == false)
        {
            LogHandler.SendMessage($"[{name}] can't found Collider2D on leftSword", MessageType.Warning);
        }
        
        if (rightSword?.TryGetComponent(out _rightCollider) == false)
        {
            LogHandler.SendMessage($"[{name}] can't found Collider2D on rightSword", MessageType.Warning);
        }
    }

    private void OnEnable()
    {
        inputReader.attackPerformedEvent += OnAttack;
        animationEventsReceiver.drawSwords += DrawSwords;
        animationEventsReceiver.sheatheSwords += SheatheSwords;
    }

    private void OnDisable()
    {
        inputReader.attackPerformedEvent -= OnAttack;
        animationEventsReceiver.drawSwords -= DrawSwords;
        animationEventsReceiver.sheatheSwords -= SheatheSwords;
    }

    // Called when the player attacks
    private void OnAttack()
    {
        // Can't attack while doing something else or not grounded
        if (_playerMovement.IsDashing || _playerMovement.IsHooking || _playerMovement.IsPullingBlock || !_playerMovement.isGrounded)
            return;

        ResetSheathingTimer();

        // If the swords are sheathed, draw them
        if (_sheathed)
        {
            animator.SetTrigger("Drawing Swords");
            _sheatheCoroutine = StartCoroutine(SheatheAfterTime(timeBeforeSheathing));
            return;
        }

        // Triggers the corresponding animation based on current attack pattern
        switch (_combatPhase)
        {
            default:
            case 0:
                animator.SetTrigger("Attack1");
                LockMovement(1f);
                _combatPhase = 1;
                break;

            case 1:
                animator.SetTrigger("Attack2");
                LockMovement(1f);
                _combatPhase = 2;
                break;

            case 2:
                animator.SetTrigger("Attack3");
                LockMovement(1.5f);
                _combatPhase = 0;
                break;
        }
    }

    // Locks all player inputs
    private void LockMovement(float time)
    {
        inputReader.DisableMove();
        inputReader.DisableJump();
        inputReader.DisableDash();
        inputReader.DisableAttack();
        inputReader.DisableSandSoldier();
        inputReader.DisableHookInteract();
        inputReader.DisableHookGrapple();

        StartCoroutine(UnlockMovement(time));
    }

    // Unlocks all player inputs after a time
    private IEnumerator UnlockMovement(float time)
    {
        yield return new WaitForSeconds(time);

        inputReader.EnableMove();
        inputReader.EnableJump();
        inputReader.EnableDash();
        inputReader.EnableAttack();
        inputReader.EnableSandSoldier();
        inputReader.EnableHookInteract();
        inputReader.EnableHookGrapple();
    }

    // Resets the sheathing timer, used when not attacking to automatically sheathe back the weapons
    public void ResetSheathingTimer()
    {
        if (_sheatheCoroutine != null)
        {
            StopCoroutine(_sheatheCoroutine);
            _sheatheCoroutine = StartCoroutine(SheatheAfterTime(timeBeforeSheathing));
        }
    }

    // Sheathes the weapons after a time
    private IEnumerator SheatheAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        animator.SetTrigger("Sheathing Swords");
    }

    private void SetupLocal(Transform sword, Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        sword.parent = parent;
        sword.localPosition = localPosition;
        sword.localRotation = localRotation;
    }

    // Draws the swords, switching sockets
    public void DrawSwords()
    {
        SetupLocal(rightSword, rightSocket.hand, Vector3.zero, Quaternion.identity);
        SetupLocal(leftSword, leftSocket.hand, Vector3.zero, Quaternion.identity);
        _rightCollider.enabled = true;
        _leftCollider.enabled = true;
        
        _sheathed = false;
        animator.SetFloat("Sheathed", 0f);
    }

    // Sheathes the swords, switching sockets
    public void SheatheSwords()
    {
        SetupLocal(rightSword, rightSocket.back, Vector3.zero, Quaternion.identity);
        SetupLocal(leftSword, leftSocket.back, Vector3.zero, Quaternion.identity);
        _rightCollider.enabled = false;
        _leftCollider.enabled = false;

        _sheathed = true;
        animator.SetFloat("Sheathed", 1f);
    }

}
