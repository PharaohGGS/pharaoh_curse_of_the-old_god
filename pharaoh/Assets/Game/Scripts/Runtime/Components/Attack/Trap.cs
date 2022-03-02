using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class Trap : MonoBehaviour
    {
        public void PlayAttackAnimation(Damager damager)
        {
            if (!TryGetComponent(out Damager trap)) return;
            if (!TryGetComponent(out Animator animator)) return;
            if (damager != trap) return;

            animator.ResetTrigger("isAttacking");
            animator.SetTrigger("isAttacking");
        }
    }
}