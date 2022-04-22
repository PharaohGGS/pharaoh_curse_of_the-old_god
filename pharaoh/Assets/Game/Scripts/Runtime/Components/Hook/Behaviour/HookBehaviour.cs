using System;
using System.Collections;
using System.Collections.Generic;
using Pharaoh.Tools.Inputs;
using PlayerMovement = Pharaoh.Gameplay.Components.Movement.PlayerMovement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.VFX.Utility;

namespace Pharaoh.Gameplay
{
    public abstract class HookBehaviour : MonoBehaviour
    {
        [SerializeField, Tooltip("Inputs handler")]
        protected InputReader inputs;

        [SerializeField, Tooltip("Events handler")]
        protected HookBehaviourEvents events;
        
        protected HookCapacity _hook;
        protected RaycastHit2D[] _hits;

        public bool isCurrentTarget { get; protected set; }
        public Vector2 nextPosition { get; protected set; }

        public UnityEvent onFoundTarget;
        public UnityEvent onLoosingTarget;
        
        protected virtual void Awake()
        {
            _hook = null;
            _hits = new RaycastHit2D[2];
        }

        public virtual void FoundBestTarget(HookCapacity hook, GameObject target)
        {
            if (target == gameObject)
            {
                onFoundTarget?.Invoke();
            }
            else
            {
                onLoosingTarget?.Invoke();
            }
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