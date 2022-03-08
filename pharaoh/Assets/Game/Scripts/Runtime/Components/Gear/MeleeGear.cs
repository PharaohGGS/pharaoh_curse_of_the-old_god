using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Damager))]
    public class MeleeGear : Gear<MeleeGearData>
    {
        public Damager damager { get; private set; }
        
        [HideInInspector] public UnityEvent onWeaponThrown = new UnityEvent();

        protected override void Awake()
        {
            base.Awake();
            damager = TryGetComponent(out Damager d) ? d : null;
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