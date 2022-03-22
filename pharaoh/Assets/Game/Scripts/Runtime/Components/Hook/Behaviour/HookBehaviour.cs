using System;
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

        [Tooltip("Event when the behaviour start")]
        public static event Action<HookBehaviour> started;
        [Tooltip("Event when the behaviour is performing an action")]
        public static event Action<HookBehaviour> performed;
        [Tooltip("Event when the behaviour ended")]
        public static event Action<HookBehaviour> ended;
        [Tooltip("Event when the behaviour is released")] 
        public static event Action<HookBehaviour> released;

        protected PlayerInput _input;
        protected HookCapacity _hook;

        public bool isCurrentTarget { get; protected set; }
        public Vector2 nextPosition { get; protected set; }
        
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
            isCurrentTarget = target == gameObject;
            _hook = isCurrentTarget ? hook : null;
            started?.Invoke(this);
        }

        public virtual void Perform()
        {
            performed?.Invoke(this);
        }

        public virtual void End()
        {
            ended?.Invoke(this);
        }

        public virtual void Release()
        {
            released?.Invoke(this);
            isCurrentTarget = false;
        }
    }
}