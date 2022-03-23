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
        [SerializeField, Tooltip("Event when hooking for interaction to gameobject")]
        private UnityEvent<HookCapacity, GameObject> onHookInteract = new UnityEvent<HookCapacity, GameObject>();
        [SerializeField, Tooltip("Event when hooking for grappling to gameobject")]
        private UnityEvent<HookCapacity, GameObject> onHookGrapple = new UnityEvent<HookCapacity, GameObject>();
        
        [SerializeField] private InputReader inputs;
        [SerializeField] private HookBehaviourEvents events;

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
            if (events) events.released += OnHookReleased;

            // Movement's events binding
            if (inputs)
            {
                inputs.hookGrapplePerformedEvent += OnHookGrapple;
                inputs.hookInteractPerformedEvent += OnHookInteract;
            }
        }

        private void OnDisable()
        {
            // Hook bindings
            if (events) events.released -= OnHookReleased;

            // Movement's events binding
            if (inputs)
            {
                inputs.hookGrapplePerformedEvent -= OnHookGrapple;
                inputs.hookInteractPerformedEvent -= OnHookInteract;
            }
        }
        
        private void Update()
        {
            SearchTargets();

            _potentialTarget = _movement.IsFacingRight switch
            {
                //facing right with right target
                true when _bestTargetRight != null => _bestTargetRight,
                //facing right without right target
                true when _bestTargetRight == null && _bestTargetLeft != null => _bestTargetLeft,
                //facing left with left target
                false when _bestTargetLeft != null => _bestTargetLeft,
                //facing left without left target
                false when _bestTargetLeft == null && _bestTargetRight != null => _bestTargetRight,
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
        
        private void OnHookInteract()
        {
            // unhook the current hooked object if there is one
            if (_currentTarget) Release();
            // hook the nearest hookable objects if there is one
            if (!_potentialTarget) return;
            
            _currentTarget = _potentialTarget;
            
            // select the target based on the direction the player's facing
            Debug.Log($"hooking to {_currentTarget.name}");
            onHookInteract?.Invoke(this, _currentTarget);
        }
        private void OnHookGrapple()
        {
            // unhook the current hooked object if there is one
            if (_currentTarget) Release();
            // hook the nearest hookable objects if there is one
            if (!_potentialTarget) return;
            
            _currentTarget = _potentialTarget;
            
            // select the target based on the direction the player's facing
            Debug.Log($"hooking to {_currentTarget.name}");
            onHookGrapple?.Invoke(this, _currentTarget);
        }
        
        private void OnHookReleased(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;

            Release();
        }

        #region Editor Debug

#if UNITY_EDITOR
        protected void OnDrawGizmos()
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
            Vector2 DirectionFromAngle(float eulerY, float degreeAngle)
            {
                degreeAngle += eulerY;
                return new Vector2(Mathf.Sin(degreeAngle * Mathf.Deg2Rad), Mathf.Cos(degreeAngle * Mathf.Deg2Rad));
            }

            // Draws a disc around the player displaying the targeting range
            UnityEditor.Handles.color = new Color(1f, 0.7531517f, 0f, 1f);
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, overlapingRadius);

            if (!inputs) return;
            
            float eulerY = -transform.eulerAngles.z + 90f * (inputs.isFacingRight ? 1f : -1f);
            Vector3 angle0 = DirectionFromAngle(eulerY, -overlapingFov / 2);
            Vector3 angle1 = DirectionFromAngle(eulerY, overlapingFov / 2);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + angle0 * overlapingRadius);
            Gizmos.DrawLine(transform.position, transform.position + angle1 * overlapingRadius);

            if (_overlaps.Length <= 0) return;

            Gizmos.color = Color.green;

            foreach (var overlap in _overlaps)
            {
                if (overlap == null) continue;
                Gizmos.DrawLine(transform.position, overlap.transform.position);
            }
        }
#endif

        #endregion
    }
}