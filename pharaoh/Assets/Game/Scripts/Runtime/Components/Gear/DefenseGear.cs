using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Damager))]
    public class DefenseGear : Gear<DefenseGearData>
    {
        public Damager damager { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            damager = TryGetComponent(out Damager d) ? d : null;
        }
    }
}