using System;
using Pharaoh.Tools.Inputs;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class HookBehaviour : MonoBehaviour
    {
        [SerializeField]
        protected InputReader _input;

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

        protected HookCapacity _hook;
        protected RaycastHit2D[] _hits;

        public bool isCurrentTarget { get; protected set; }
        public Vector2 nextPosition { get; protected set; }
        
        protected virtual void Awake()
        {
            _hook = null;
            _hits = new RaycastHit2D[2];
            if (hookIndicator) hookIndicator.SetActive(false);
        }

        public virtual void FoundBestTarget(TargetFinder finder, GameObject target)
        {
            if (hookIndicator) hookIndicator.SetActive(target == gameObject);
        }

        public virtual void Interact(HookCapacity hook, GameObject target)
        {
            isCurrentTarget = target == gameObject;
            _hook = isCurrentTarget ? hook : null;
            started?.Invoke(this);
        }
        
        protected virtual void Perform() => performed?.Invoke(this);

        protected virtual void End() => ended?.Invoke(this);

        protected virtual void Release()
        {
            released?.Invoke(this);
            isCurrentTarget = false;
        }
    }
}