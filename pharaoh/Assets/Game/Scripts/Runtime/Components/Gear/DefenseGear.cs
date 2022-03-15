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
            coll2D.enabled = false;
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

        public void Repel(Gear gear)
        {
            if (gear != this) return;

            Debug.Log($"Repel with {name}");
        }
    }
}