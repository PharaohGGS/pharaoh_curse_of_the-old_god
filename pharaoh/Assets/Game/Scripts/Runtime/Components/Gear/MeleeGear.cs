using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Damager))]
    public class MeleeGear : Gear<MeleeGearData>
    {
        public Damager damager { get; private set; }
        
        [HideInInspector] public UnityEvent onWeaponThrown = new UnityEvent();

        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();
            damager = TryGetComponent(out Damager d) ? d : null;
            if (!TryGetComponent(out _animator))
            {
                LogHandler.SendMessage($"{name} can't play animation", MessageType.Warning);
            }

            SetAttackState();
        }

        public void Stab(Gear gear)
        {
            if (gear != this || !_animator) return;
            
            _animator.ResetTrigger("isAttacking");
            _animator.SetTrigger("isAttacking");
        }

        /// <summary>
        /// set enabled to all necessary component for killing
        /// </summary>
        /// <param name="value">int instead of bool 0 = false > 0 = true</param>
        public void SetAttackState(int value = 0)
        {
            if (!coll2D) return;
            coll2D.enabled = value > 0;
        }

        public void Throw(Gear gear)
        {
            if (this != gear || !TryGetComponent(out Ballistic ballistic)) return;
            if (GetData()?.throwable == false) return;

            transform.parent = null;
            onWeaponThrown?.Invoke();
        }
    }
}