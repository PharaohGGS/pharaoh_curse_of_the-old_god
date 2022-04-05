using PlayerMovement = Pharaoh.Gameplay.Components.Movement.PlayerMovement;
using System.Collections;
using UnityEngine;

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

    public Transform rightHand;
    public Transform leftHand;
    public Transform rightSocket;
    public Transform leftSocket;
    public Transform rightSword;
    public Transform leftSword;

    [Header("Variables")]

    public float timeBeforeSheathing = 5f;


    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
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

        Debug.Log("Sheathing");
        animator.SetTrigger("Sheathing Swords");
    }

    // Draws the swords, switching sockets
    public void DrawSwords()
    {
        rightSword.parent = rightHand;
        rightSword.localPosition = Vector3.zero;
        rightSword.localRotation = Quaternion.identity;
        leftSword.parent = leftHand;
        leftSword.localPosition = Vector3.zero;
        leftSword.localRotation = Quaternion.identity;

        _sheathed = false;
        animator.SetFloat("Sheathed", 0f);
    }

    // Sheathes the swords, switching sockets
    public void SheatheSwords()
    {
        rightSword.parent = rightSocket;
        rightSword.localPosition = Vector3.zero;
        rightSword.localRotation = Quaternion.identity;
        leftSword.parent = leftSocket;
        leftSword.localPosition = Vector3.zero;
        leftSword.localRotation = Quaternion.identity;

        _sheathed = true;
        animator.SetFloat("Sheathed", 1f);
    }

}
