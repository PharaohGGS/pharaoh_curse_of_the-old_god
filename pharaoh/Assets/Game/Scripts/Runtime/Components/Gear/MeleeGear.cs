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
        
        public void Throw(Transform target)
        {
            if (!_rigidbody2D) return;
            var initialVelocity = GetData().throwableInitialVelocity;
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