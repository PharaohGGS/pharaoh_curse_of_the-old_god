using System;
using Pharaoh.Tools.Inputs;
using PlayerMovement = Pharaoh.Gameplay.Components.Movement.PlayerMovement;
using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class HookBehaviour : MonoBehaviour
    {
        [SerializeField, Tooltip("Inputs handler")]
        protected InputReader inputs;

        [SerializeField, Tooltip("Events handler")]
        protected HookBehaviourEvents events;

        [SerializeField, Tooltip("FX hookIndicator for the best target selected")] 
        protected GameObject hookIndicator;
        
        protected HookCapacity _hook;
        protected RaycastHit2D[] _hits;

        protected PlayerMovement _playerMovement; //need this to manage the hooking/hooked animations

        public bool isCurrentTarget { get; protected set; }
        public Vector2 nextPosition { get; protected set; }
        
        protected virtual void Awake()
        {
            _hook = null;
            _hits = new RaycastHit2D[2];
            if (hookIndicator) hookIndicator.SetActive(false);

            _playerMovement = FindObjectOfType<PlayerMovement>();
        }

        public virtual void FoundBestTarget(HookCapacity hook, GameObject target)
        {
            if (hookIndicator) hookIndicator.SetActive(target == gameObject);
        }

        public virtual void Interact(HookCapacity hook, GameObject target)
        {
            isCurrentTarget = target == gameObject;
            _hook = isCurrentTarget ? hook : null;
            events?.Started(this);
        }
        
        public virtual void Perform() => events?.Performed(this);
        
        public virtual void End() => events?.Ended(this);
        
        public virtual void Release()
        {
            events?.Released(this);
            isCurrentTarget = false;
        }
    }
}