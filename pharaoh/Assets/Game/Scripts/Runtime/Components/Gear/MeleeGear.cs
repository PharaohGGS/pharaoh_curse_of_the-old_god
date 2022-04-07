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

        public void Attack(Transform target)
        {
            var data = GetData();
            var distance = Mathf.Abs(target.position.x - transform.position.x);

            var canThrow = data.throwable;
            var isThrowable = distance <= data.throwableRange;
            var isStabbable = distance <= data.range;


            switch (canThrow)
            {
                case false when isStabbable:
                case true when isStabbable && isThrowable:
                    Stab(target);
                    break;
                case true when !isStabbable && isThrowable:
                    Throw(target);
                    break;
                default:
                    LogHandler.SendMessage($"{name} is too far from {target.name}", MessageType.Warning);
                    break;
            }
        }

        private void Stab(Transform target)
        {
            LogHandler.SendMessage($"{name} stabbing {target}", MessageType.Log);
            if (!target || !_animator) return;

            _animator.ResetTrigger("isAttacking");
            _animator.SetTrigger("isAttacking");
        }

        private void Throw(Transform target)
        {
            var data = GetData();
            if (!target || !data || !data.throwable || !_rigidbody2D) return;

            var initialVelocity = data.throwableInitialVelocity;
            var direction = (target.position + Vector3.up) - transform.position;
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody2D.AddForce(direction.normalized * initialVelocity, ForceMode2D.Impulse);
            SocketAttach(false);
            onWeaponThrown?.Invoke();
        }

        public override float GetRate()
        {
            if (TryGetData(out MeleeGearData melee) && melee.throwable && isThrown)
            {
                return melee.throwablePickingTime;
            }

            return base.GetRate();
        }
    }
}