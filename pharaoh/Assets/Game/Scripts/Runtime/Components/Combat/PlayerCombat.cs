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
    private Coroutine _combatPhaseReset;

    [Header("Components")]

    public InputReader inputReader;
    public Animator animator;

    [Header("Sockets")]
    
    public Transform rightSword;
    public Sockets rightSocket;

    public Transform leftSword;
    public Sockets leftSocket;
    
    [Header("Variables")]

    public float timeBeforeSheathing = 5f;


    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        inputReader.attackPerformedEvent += OnAttack;
    }

    private void OnDisable()
    {
        inputReader.attackPerformedEvent -= OnAttack;
    }

    // Called when the player attacks
    private void OnAttack()
    {
        // Can't attack while doing something else or not grounded
        if (_playerMovement.IsDashing || _playerMovement.IsHooking || _playerMovement.IsPullingBlock || !_playerMovement.IsGrounded)
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
                if (_combatPhaseReset != null)
                    StopCoroutine(_combatPhaseReset);
                animator.SetTrigger("Attack1");
                _combatPhaseReset = StartCoroutine(ResetCombatPhase(1.5f));
                LockMovement(1f);
                _combatPhase = 1;
                break;

            case 1:
                if (_combatPhaseReset != null)
                    StopCoroutine(_combatPhaseReset);
                animator.SetTrigger("Attack2");
                _combatPhaseReset = StartCoroutine(ResetCombatPhase(1.5f));
                LockMovement(1f);
                _combatPhase = 2;
                break;

            case 2:
                if (_combatPhaseReset != null)
                    StopCoroutine(_combatPhaseReset);
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
        inputReader.EnableAttack();
        inputReader.EnableSandSoldier();
        inputReader.EnableHookInteract();
        inputReader.EnableHookGrapple();
    }

    // Resets the combat phase after a time
    private IEnumerator ResetCombatPhase(float time)
    {
        yield return new WaitForSeconds(time);

        Debug.Log("Reset");
        _combatPhase = 0;
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
        
        _sheathed = false;
        animator.SetFloat("Sheathed", 0f);
    }

    // Sheathes the swords, switching sockets
    public void SheatheSwords()
    {
        SetupLocal(rightSword, rightSocket.back, Vector3.zero, Quaternion.identity);
        SetupLocal(leftSword, leftSocket.back, Vector3.zero, Quaternion.identity);

        _sheathed = true;
        animator.SetFloat("Sheathed", 1f);
    }

}
