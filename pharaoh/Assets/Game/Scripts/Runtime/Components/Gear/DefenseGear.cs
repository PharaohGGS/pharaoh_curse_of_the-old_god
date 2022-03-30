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

        public void Repel(Gear gear, GameObject target)
        {
            if (gear != this) return;

            Debug.Log($"Repel with {name}");
        }
        
        private void OnHookStarted(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            if (!TryGetComponent(out Rigidbody2D rb2D)) return;
            
            rb2D.velocity = Vector2.zero;
            rb2D.bodyType = RigidbodyType2D.Kinematic;
        }

        private void OnHookPerformed(HookBehaviour behaviour)
        { 
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            if (!TryGetComponent(out Rigidbody2D rb2D)) return;

            transform.parent = null;
            rb2D.MovePosition(behaviour.nextPosition);
        }

        private void OnHookEnded(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            if (!TryGetComponent(out Rigidbody2D rb2D)) return;

            rb2D.bodyType = RigidbodyType2D.Dynamic;
        }
    
        private void OnHookReleased(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget || behaviour.gameObject != gameObject) return;
            if (behaviour is not SnatchHookBehaviour snatch) return;
            if (!TryGetComponent(out Rigidbody2D rb2D)) return;

            rb2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}