using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay.Components
{
    public class DefenseGear : Gear<DefenseGearData>, IArmor
    {
        [SerializeField, Header("HookEvents")]
        private HookBehaviourEvents hookEvents;
        
        private void OnEnable()
        {
            // Hook bindings
            if (!hookEvents) return;
            hookEvents.started += OnHookStarted;
            hookEvents.performed += OnHookPerformed;
            hookEvents.ended += OnHookEnded;
            hookEvents.released += OnHookReleased;
        }

        private void OnDisable()
        {
            // Hook bindings
            if (!hookEvents) return;
            hookEvents.started -= OnHookStarted;
            hookEvents.performed -= OnHookPerformed;
            hookEvents.ended -= OnHookEnded;
            hookEvents.released -= OnHookReleased;
        }
        
        private void OnHookStarted(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            if (!_rigidbody2D) return;
            
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        private void OnHookPerformed(HookBehaviour behaviour)
        { 
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            if (!_rigidbody2D) return;

            transform.parent = null;
            _rigidbody2D.MovePosition(behaviour.nextPosition);
        }

        private void OnHookEnded(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            if (!_rigidbody2D) return;

            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    
        private void OnHookReleased(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            if (!_rigidbody2D) return;

            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }

        public void Defend(GameObject target)
        {
            Repel(target);
        }

        public void Repel(GameObject target)
        {
            Debug.Log($"Repel with {name}");
        }
    }
}