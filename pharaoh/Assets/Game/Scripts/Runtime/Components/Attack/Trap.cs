using UnityEngine;

namespace Pharaoh.Gameplay.Components
{
    public class Trap : MonoBehaviour
    {
        public void PlayAttackAnimation(Gear gear)
        {
            if (!TryGetComponent(out Animator animator)) return;
            if (!TryGetComponent(out Gear thisGear) || gear != thisGear) return;

            animator.ResetTrigger("isAttacking");
            animator.SetTrigger("isAttacking");
        }
    }
}