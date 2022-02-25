using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class Trap : Damager
    {
        public void PlayAttackAnimation(Damager damager)
        {
            if (damager != this || !TryGetComponent(out Animator animator)) return;

            animator.ResetTrigger("isAttacking");
            animator.SetTrigger("isAttacking");
        }
    }
}