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
        private Transform _currentTarget;

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
        /// to be used by the AnimationEvent
        /// </summary>
        /// <param name="value">int instead of bool 0 = false > 0 = true</param>
        public void SetAttackState(int value = 0)
        {
            if (!coll2D) return;
            coll2D.enabled = value > 0;
        }

        public void SetupAttack(Gear attackingGear, Transform target)
        {
            if (attackingGear != this || !target) return;

            _currentTarget = target;
        }

        public void Throw(Gear gear)
        {
            if (this != gear || GetData()?.throwable == false) return;

            if (rb2D)
            {
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                var direction = (Vector2)_currentTarget.position - rb2D.position; 
                rb2D.AddForce(direction.normalized * GetData().throwableInitialVelocity, ForceMode2D.Impulse);
            }

            transform.parent = null;
            onWeaponThrown?.Invoke();
        }
    }
}