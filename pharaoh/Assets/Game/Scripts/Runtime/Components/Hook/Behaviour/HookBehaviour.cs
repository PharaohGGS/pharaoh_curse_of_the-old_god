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
        
        [SerializeField, Tooltip("wait time loosing target")]
        protected float waitTimeLoosing;

        protected HookCapacity _hook;
        protected RaycastHit2D[] _hits;

        protected PlayerMovement _playerMovement; //need this to manage the hooking/hooked animations

        public bool isCurrentTarget { get; protected set; }
        public Vector2 nextPosition { get; protected set; }

        public UnityEvent onFoundTarget;
        public UnityEvent onLoosingTarget;
        public UnityEvent onLostTarget;
        public UnityEvent onInteract;
        
        protected virtual void Awake()
        {
            _hook = null;
            _hits = new RaycastHit2D[2];

            _playerMovement = FindObjectOfType<PlayerMovement>();
        }

        public virtual void FoundBestTarget(HookCapacity hook, GameObject target)
        {
            StartCoroutine(ActivateIndicator(target == gameObject));
        }

        protected IEnumerator ActivateIndicator(bool activate)
        {
            if (activate)
            {
                onFoundTarget?.Invoke();
            }
            else
            {
                onLoosingTarget?.Invoke();
                yield return new WaitForSeconds(waitTimeLoosing);
                onLostTarget?.Invoke();
            }
        }

        public virtual void Interact(HookCapacity hook, GameObject target)
        {
            isCurrentTarget = target == gameObject;
            _hook = isCurrentTarget ? hook : null;
            if (isCurrentTarget) onInteract?.Invoke();
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