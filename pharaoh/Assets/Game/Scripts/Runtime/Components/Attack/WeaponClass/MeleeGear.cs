using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Damager))]
    public class MeleeGear : Gear<MeleeGearData>
    {
        public Damager damager { get; private set; }

        public UnityEvent onWeaponThrown = new UnityEvent();

        protected override void Awake()
        {
            base.Awake();
            damager = TryGetComponent(out Damager d) ? d : null;
        }

        public void Throw(Damager thrownDamager)
        {
            if (thrownDamager != damager || !GetData().throwable || 
                !TryGetComponent(out Ballistic ballistic)) return;

            transform.parent = null;
            onWeaponThrown?.Invoke();
        }
    }
}