using System;
using Pharaoh.Tools;
using Pharaoh.Tools.Debug;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Damager))]
    public class MeleeGear : Gear<MeleeGearData>, IWeapon
    {
        [HideInInspector] public UnityEvent onWeaponThrown = new UnityEvent();

        private Animator _animator;
        private Transform _currentTarget;

        protected override void Awake()
        {
            base.Awake();
            if (!TryGetComponent(out _animator))
            {
                LogHandler.SendMessage($"{name} can't play animation", MessageType.Warning);
            }
        }

        public void Attack(GameObject target)
        {
            if (GetData().throwable) Throw(target); 
            else Stab(target);
        }

        private void Stab(GameObject target)
        {
            if (!target || !_animator) return;

            _animator.ResetTrigger("isAttacking");
            _animator.SetTrigger("isAttacking");
        }

        private void Throw(GameObject target)
        {
            if (!target || GetData()?.throwable == false || !_rigidbody2D) return;

            //float speed = GetData().throwableInitialVelocity;
            float gravity = Physics2D.gravity.magnitude;
            Vector2 targetPosition = target.transform.position;
            Vector2 position = _rigidbody2D.position;
            float height = position.y;
            LaunchData data = LaunchData.Calculate(gravity, height, targetPosition, position/*, speed*/);

            _rigidbody2D.velocity = data.initialVelocity;
            SocketAttach(false);
            onWeaponThrown?.Invoke();
        }
    }
}