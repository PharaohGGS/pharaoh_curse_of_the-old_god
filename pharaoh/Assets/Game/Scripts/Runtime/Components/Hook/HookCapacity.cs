using System;
using Pharaoh.Gameplay.Components.Movement;
using Pharaoh.Tools.Debug;
using Pharaoh.Tools.Inputs;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    /// <summary>
    /// Capacity in case of hook to some gameobject
    /// In charge of hooking & cancelling hook
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    public abstract class HookCapacity : TargetFinder
    {
        [SerializeField] protected bool discardBehindOverlap;

        protected PlayerInput _input;
        protected PlayerMovement _movement;
        protected GameObject _potentialTarget;

        public UnityEvent<HookCapacity> onHookRelease = new UnityEvent<HookCapacity>();

        protected override void Awake()
        {
            base.Awake();
            _input = new PlayerInput();
            _movement = GetComponent<PlayerMovement>();
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
        
        protected virtual void Update()
        {
            SearchTargets();

            _potentialTarget = _movement.IsFacingRight switch
            {
                //facing right with right target
                true when _bestTargetRight != null => _bestTargetRight,
                //facing right without right target
                true when _bestTargetRight == null && _bestTargetLeft != null && !discardBehindOverlap => _bestTargetLeft,
                //facing left with left target
                false when _bestTargetLeft != null => _bestTargetLeft,
                //facing left without left target
                false when _bestTargetLeft == null && _bestTargetRight != null && !discardBehindOverlap => _bestTargetRight,
                // else
                _ => null
            };
            
            onFoundBestTarget?.Invoke(this, _potentialTarget);
        }

        protected virtual void OnRelease()
        {
            onHookRelease?.Invoke(this);
            if (!_currentTarget) return;

            Debug.Log($"unhooking from {_currentTarget.name}");
            _currentTarget = null;
        }
    }
}