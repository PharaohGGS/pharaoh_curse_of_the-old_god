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
    public class HookCapacity : MonoBehaviour
    {
        [Header("Detection")]

        [SerializeField, Tooltip("Overlap detectable colliders"), Range(2, 20)] 
        private int overlapCount = 5;
        [SerializeField, Tooltip("TargetFinder radius"), Range(2f, 20f)] 
        private float overlappingRadius = 6f;
        [SerializeField, Tooltip("TargetFinder FOV"), Range(1f, 360f)] 
        private float overlappingFov = 270f;
        [SerializeField, Tooltip("Target layers")]
        protected LayerMask whatIsTarget;
        [field: SerializeField, Tooltip("Obstacle layers")]
        public LayerMask whatIsObstacle { get; protected set; }

        [Header("Capacity Events")]

        [SerializeField, Tooltip("Event when found the best target")]
        private UnityEvent<HookCapacity, GameObject> onFoundBestTarget = new UnityEvent<HookCapacity, GameObject>();
        [SerializeField, Tooltip("Event when hooking for interaction to gameobject")]
        private UnityEvent<HookCapacity, GameObject> onHookInteract = new UnityEvent<HookCapacity, GameObject>();
        [SerializeField, Tooltip("Event when hooking for grappling to gameobject")]
        private UnityEvent<HookCapacity, GameObject> onHookGrapple = new UnityEvent<HookCapacity, GameObject>();
        
        [SerializeField, Header("Input Reader")] private InputReader inputs;
        [SerializeField, Header("Player Skills")] private PlayerSkills skills;
        [SerializeField, Header("Hook Behaviour Events")] private HookBehaviourEvents events;

        [Header("Actions Data")] 
        
        [SerializeField, Tooltip("Data for the pull action")]
        private PullHookData pullHookData;
        public PullHookData pullData => pullHookData;

        [SerializeField, Tooltip("Data for the grab action")]
        private SnatchHookData snatchHookData;
        public SnatchHookData snatchData => snatchHookData;

        [SerializeField, Tooltip("Data for the grapple action")]
        private GrappleHookData grappleHookData;
        public GrappleHookData grappleData => grappleHookData;
        
        private PlayerMovement _movement;
        private GameObject _potentialTarget;

        private Collider2D[] _overlaps;

        private GameObject _bestTargetRight;
        private GameObject _bestTargetLeft;
        private HookBehaviour _currentBehaviour;

        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        public Vector2 center { get; private set; }

        private void Awake()
        {
            _overlaps = new Collider2D[overlapCount];
            _movement = GetComponent<PlayerMovement>();
            
            if (!TryGetComponent(out _rigidbody))
            {
                LogHandler.SendMessage($"No rigidbody to center", MessageType.Warning);
            }
            
            if (!TryGetComponent(out _collider))
            {
                LogHandler.SendMessage($"No collider to center", MessageType.Warning);
            }
        }

        private void OnEnable()
        {
            // Hook bindings
            if (events)
            {
                events.started += OnHookStarted;
                events.released += OnHookReleased;
            }

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
            if (events)
            {
                events.started -= OnHookStarted;
                events.released -= OnHookReleased;
            }

            // Movement's events binding
            if (inputs)
            {
                inputs.hookGrapplePerformedEvent -= OnHookGrapple;
                inputs.hookInteractPerformedEvent -= OnHookInteract;
            }
        }
        
        private void Update()
        {
            if (!skills.HasGrapplingHook) return;

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

        private void SearchTargets()
        {
            center = _rigidbody.position + _collider.offset;
            int overlapCount = Physics2D.OverlapCircleNonAlloc(center, overlappingRadius, _overlaps, whatIsTarget);

            float closestDistanceRight = Mathf.Infinity;
            float closestDistanceLeft = Mathf.Infinity;

            int bestOverlapRight = -1;
            int bestOverlapLeft = -1;

            // Loops each target and remove those behind walls as well as selects the closest one
            // Selects the best target to the right of the player and to the left
            for (int overlapIndex = 0; overlapIndex < overlapCount; overlapIndex++)
            {
                var overlap = _overlaps[overlapIndex];

                //if (_currentTarget == overlap.gameObject) continue;

                Vector2 direction = (Vector2)overlap.transform.position - center;
                float distance = direction.magnitude;
                
                if (Vector2.Angle(transform.right * (inputs.isFacingRight ? 1 : -1), direction.normalized) >= overlappingFov / 2) continue;
                if (Physics2D.RaycastAll(center, direction.normalized, distance, whatIsObstacle).Length > 0) continue;

                switch (overlap.transform.position.x > center.x)
                {
                    case true when distance < closestDistanceRight:
                        bestOverlapRight = overlapIndex;
                        closestDistanceRight = distance;
                        break;
                    case false when distance < closestDistanceLeft:
                        bestOverlapLeft = overlapIndex;
                        closestDistanceLeft = distance;
                        break;
                }
            }

            // Selects the best targets if there is
            _bestTargetRight = bestOverlapRight == -1 ? null : _overlaps[bestOverlapRight].gameObject;
            _bestTargetLeft = bestOverlapLeft == -1 ? null : _overlaps[bestOverlapLeft].gameObject;
        }

        public void Release()
        {
            if (!_currentBehaviour) return;
            _currentBehaviour.Release();
        }
        
        private void OnHookInteract()
        {
            // unhook the current hooked object if there is one
            Release();
            // hook the nearest hookable objects if there is one
            if (!_potentialTarget) return;
            // select the target based on the direction the player's facing
            onHookInteract?.Invoke(this, _potentialTarget);
        }

        private void OnHookGrapple()
        {
            // unhook the current hooked object if there is one
            //if (_currentTarget) Release();
            // hook the nearest hookable objects if there is one
            if (!_potentialTarget) return;
            // select the target based on the direction the player's facing
            onHookGrapple?.Invoke(this, _potentialTarget);
        }

        private void OnHookStarted(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;
            _currentBehaviour = behaviour;
        }
        
        private void OnHookReleased(HookBehaviour behaviour)
        {
            if (!behaviour.isCurrentTarget) return;
            Debug.Log($"release from {behaviour.name}");
            _currentBehaviour = null;
        }

        #region Editor Debug

#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            if (!_movement) return;
            if (!TryGetComponent(out Collider2D coll)) return;

            // Draws the best target to the right(red if not the faced direction)
            Gizmos.color = _movement.IsFacingRight
                ? new Color(1f, 0.7531517f, 0f, 1f)
                : new Color(1f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetRight != null)
                Gizmos.DrawLine(transform.position + (Vector3)coll.offset, _bestTargetRight.transform.position);

            // Draws the best target to the left (red if not the faced direction)
            Gizmos.color = !_movement.IsFacingRight
                ? new Color(1f, 0.7531517f, 0f, 1f)
                : new Color(1f, 0.7531517f, 0f, 0.1f);
            if (_bestTargetLeft != null)
                Gizmos.DrawLine(transform.position + (Vector3)coll.offset, _bestTargetLeft.transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 DirectionFromAngle(float eulerY, float degreeAngle)
            {
                degreeAngle += eulerY;
                return new Vector2(Mathf.Sin(degreeAngle * Mathf.Deg2Rad), Mathf.Cos(degreeAngle * Mathf.Deg2Rad));
            }

            if (!TryGetComponent(out Collider2D coll)) return;

            // Draws a disc around the player displaying the targeting range
            UnityEditor.Handles.color = new Color(1f, 0.7531517f, 0f, 1f);
            UnityEditor.Handles.DrawWireDisc(transform.position + (Vector3)coll.offset, Vector3.forward, overlappingRadius);

            if (!inputs) return;
            
            float eulerY = -transform.eulerAngles.z + 90f * (inputs.isFacingRight ? 1f : -1f);
            Vector3 angle0 = DirectionFromAngle(eulerY, -overlappingFov / 2);
            Vector3 angle1 = DirectionFromAngle(eulerY, overlappingFov / 2);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + (Vector3)coll.offset, transform.position + (Vector3)coll.offset + angle0 * overlappingRadius);
            Gizmos.DrawLine(transform.position + (Vector3)coll.offset, transform.position + (Vector3)coll.offset + angle1 * overlappingRadius);

            if (_overlaps is not {Length: > 0}) return;

            Gizmos.color = Color.green;

            foreach (var overlap in _overlaps)
            {
                if (overlap == null) continue;
                Gizmos.DrawLine(transform.position + (Vector3)coll.offset, overlap.transform.position);
            }
        }
#endif

        #endregion
    }
}