using Pharaoh.GameEvents;
using Pharaoh.Tools.Inputs;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    public abstract class HookBehaviour : MonoBehaviour
    {
        [SerializeField, Tooltip("FX hookIndicator for the best target selected")] 
        protected GameObject hookIndicator;

        [SerializeField, Tooltip("Event when the behaviour want the player to release it")] 
        protected UnityEvent onRelease = new UnityEvent();

        protected PlayerInput _input;
        protected HookCapacity _hook;

        protected bool _isCurrentTarget;
        
        protected virtual void Awake()
        {
            _hook = null;
            _input = new PlayerInput();
            hookIndicator?.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            _input.Enable();
        }

        protected virtual void OnDisable()
        {
            _input.Disable();
        }

        protected virtual void OnDestroy()
        {
            _input.Dispose();
        }

        public virtual void FoundBestTarget(TargetFinder finder, GameObject target)
        {
            hookIndicator?.SetActive(target == gameObject);
        }

        public virtual void Interact(HookCapacity hook, GameObject target)
        {
            _isCurrentTarget = target == gameObject;
            if (!_isCurrentTarget) return;
            _hook = hook;
        }

        public virtual void Release()
        {
            _isCurrentTarget = false;
            onRelease?.Invoke();
        }
    }
}