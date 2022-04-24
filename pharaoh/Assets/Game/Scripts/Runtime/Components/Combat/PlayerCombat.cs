using System;
using PlayerMovement = Pharaoh.Gameplay.Components.Movement.PlayerMovement;
using System.Collections;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

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
    private int _combatPhase = 0;
    private bool _sheathed = true;
    private Coroutine _sheatheCoroutine;
    private Coroutine _combatPhaseReset;

    [Header("Components")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Animator animator;

    [Header("Player Skills Unlocked")]
    [SerializeField] private PlayerSkills skills;

    [Header("Sockets")]
    [SerializeField] private Transform rightSword;
    [SerializeField] private Sockets rightSocket;
    [SerializeField] private Transform leftSword;
    [SerializeField] private Sockets leftSocket;

    [Header("Variables")]
    [SerializeField] private int totalCombatPhase = 3;
    [SerializeField] private float timeBeforeSheathing = 5f;

    [Header("Events")] 
    [SerializeField] private UnityEvent onSwordDash;
    [SerializeField] private UnityEvent onSwordSwarmDash;
    [SerializeField] private UnityEvent onSwordDashEnd;


    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        inputReader.attackPerformedEvent += OnAttack;
        inputReader.dashStartedEvent += OnDash;
    }
    
    private void OnDisable()
    {
        inputReader.attackPerformedEvent -= OnAttack;
        inputReader.dashStartedEvent -= OnDash;
    }

    private void OnDash()
    {
        if (!skills || _sheathed) return;
        if (skills.HasDash && !skills.HasSwarmDash) onSwordDash?.Invoke();
        if (skills.HasDash && skills.HasSwarmDash) onSwordSwarmDash?.Invoke();
    }

    public void OnDashEnd()
    {
        if (!skills || _sheathed) return;
        onSwordDashEnd?.Invoke();
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
        _combatPhase = _combatPhase switch
        {
            0 => SetCombatPhase(1f, 1.5f),
            1 => SetCombatPhase(1f, 1.5f),
            2 => SetCombatPhase(1.5f, 0.0f),
            _ => SetCombatPhase(1f, 1.5f)
        };
    }

    private int SetCombatPhase(float lockMovementTime, float resetCombatPhaseTime)
    {
        if (_combatPhaseReset != null)
        {
            StopCoroutine(_combatPhaseReset);
        }

        animator.SetTrigger($"Attack{_combatPhase + 1}");

        if (resetCombatPhaseTime <= Mathf.Epsilon)
        {
            _combatPhaseReset = StartCoroutine(ResetCombatPhase(resetCombatPhaseTime));
        }

        LockMovement(lockMovementTime);

        return (_combatPhase + 1) % totalCombatPhase;
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
