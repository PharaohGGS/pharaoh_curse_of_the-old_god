using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    [RequireComponent(typeof(Damager))]
    public class DefenseGear : Gear<DefenseGearData>
    {
        [SerializeField, Header("HookEvents")]
        private HookBehaviourEvents hookEvents;

        public Damager damager { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (!TryGetComponent(out Damager d)) return;
            damager = d;
            damager.enabled = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Hook bindings
            if (!hookEvents) return;
            hookEvents.started += OnHookStarted;
            hookEvents.performed += OnHookPerformed;
            hookEvents.ended += OnHookEnded;
            hookEvents.released += OnHookReleased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            // Hook bindings
            if (!hookEvents) return;
            hookEvents.started -= OnHookStarted;
            hookEvents.performed -= OnHookPerformed;
            hookEvents.ended -= OnHookEnded;
            hookEvents.released -= OnHookReleased;
        }

        /// <summary>
        /// set enabled to all necessary component for killing
        /// to be used by the AnimationEvent
        /// </summary>
        /// <param name="value">int instead of bool 0 = false > 0 = true</param>
        public void SetAttackState(int value = 0)
        {
            // if (!coll2D) return;
            // coll2D.enabled = value > 0;
            if (!damager) return;
            damager.enabled = value > 0;
        }

        public void Repel(Gear gear)
        {
            if (gear != this) return;

            Debug.Log($"Repel with {name}");
        }

        
        private void OnHookStarted(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            
            rb2D.velocity = Vector2.zero;
            rb2D.bodyType = RigidbodyType2D.Kinematic;
        }

        private void OnHookPerformed(HookBehaviour behaviour)
        { 
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            
            transform.parent = null;
            rb2D.MovePosition(behaviour.nextPosition);
        }

        private void OnHookEnded(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            
            rb2D.bodyType = RigidbodyType2D.Dynamic;
        }
    
        private void OnHookReleased(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            
            rb2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}