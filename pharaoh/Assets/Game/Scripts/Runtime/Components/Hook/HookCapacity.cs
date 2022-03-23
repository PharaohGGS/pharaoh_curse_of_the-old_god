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
    public class HookCapacity : TargetFinder
    {
        [SerializeField, Tooltip("Event when hooking to gameobject")]
        private UnityEvent<HookCapacity, GameObject> onHook = new UnityEvent<HookCapacity, GameObject>();

        [SerializeField] private bool discardBehindOverlap;
        [SerializeField] private InputReader _inputReader;

        [field: SerializeField, Tooltip("Data for the pull action")] 
        public PullHookData pullData { get; private set; }
        [field: SerializeField, Tooltip("Data for the grab action")] 
        public SnatchHookData snatchData { get; private set; }
        [field: SerializeField, Tooltip("Data for the grapple action")] 
        public GrappleHookData grappleData { get; private set; }
        
        private PlayerMovement _movement;
        private GameObject _potentialTarget;

        protected override void Awake()
        {
            base.Awake();
            _movement = GetComponent<PlayerMovement>();
        }

        private void OnEnable()
        {
            // Hook bindings
            HookBehaviour.released += OnHookReleased;

            // Movement's events binding
            _inputReader.hookPerformedEvent += OnHook;
        }

        private void OnDisable()
        {
            // Hook bindings
            HookBehaviour.released -= OnHookReleased;

            // Movement's events binding
            _inputReader.hookPerformedEvent -= OnHook;
        }
        
        private void Update()
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

        private void Release()
        {
            if (!_currentTarget) return;

            Debug.Log($"release from {_currentTarget.name}");
            _currentTarget = null;
        }
        
        private void OnHook()
        {
            Debug.Log($"HOOK");

            // unhook the current hooked object if there is one
            if (_currentTarget) Release();
            // hook the nearest hookable objects if there is one
            if (!_potentialTarget) return;
            
            _currentTarget = _potentialTarget;
            
            // select the target based on the direction the player's facing
            Debug.Log($"hooking to {_currentTarget.name}");
            onHook?.Invoke(this, _currentTarget);
        }
        
        private void OnHookReleased(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            Release();
        }

        #region Editor Debug

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_movement) return;

            // Draws the best target to the right(red if not the faced direction)
            Gizmos.color = _movement.isFacingRight
                ? new Color(1f, 0.7531517f, 0f, 1f)
                : new Color(1f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetRight != null)
                Gizmos.DrawLine(transform.position, _bestTargetRight.transform.position);

            // Draws the best target to the left (red if not the faced direction)
            Gizmos.color = !_movement.isFacingRight
                ? new Color(1f, 0.7531517f, 0f, 1f)
                : new Color(1f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetLeft != null)
                Gizmos.DrawLine(transform.position, _bestTargetLeft.transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            // Draws a disc around the player displaying the targeting range
            UnityEditor.Handles.color = new Color(1f, 0.7531517f, 0f, 1f);
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, overlapingRadius);
        }
#endif

        #endregion
    }
}