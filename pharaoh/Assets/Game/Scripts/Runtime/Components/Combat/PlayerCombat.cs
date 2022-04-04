using Pharaoh.Gameplay.Components.Movement;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{

    private short _combatPhase = 0;
    private PlayerMovement _playerMovement;
    private bool _sheathed = true;

    public InputReader inputReader;
    public Animator animator;
    public Transform rightHand;
    public Transform leftHand;
    public Transform rightSocket;
    public Transform leftSocket;
    public Transform rightSword;
    public Transform leftSword;


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

    private void OnAttack()
    {
        // Can't attack while doing something else or not grounded
        if (_playerMovement.IsDashing || _playerMovement.IsHooking || _playerMovement.IsPullingBlock || !_playerMovement.isGrounded)
            return;

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

    private void SwitchStance()
    {
        Debug.Log("Switching Stance");

        if (_sheathed)
            animator.SetTrigger("Draw");
        else
            animator.SetTrigger("Sheathe");

        _sheathed = !_sheathed;
    }

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

    private void DrawSwords()
    {
        rightSword.parent = rightHand;
        rightSword.localPosition = Vector3.zero;
        rightSword.localRotation = Quaternion.identity;
        leftSword.parent = leftHand;
        leftSword.localPosition = Vector3.zero;
        leftSword.localRotation = Quaternion.identity;
    }

    private void SheatheSwords()
    {
        rightSword.parent = rightSocket;
        rightSword.localPosition = Vector3.zero;
        rightSword.localRotation = Quaternion.identity;
        leftSword.parent = leftSocket;
        leftSword.localPosition = Vector3.zero;
        leftSword.localRotation = Quaternion.identity;
    }

}
