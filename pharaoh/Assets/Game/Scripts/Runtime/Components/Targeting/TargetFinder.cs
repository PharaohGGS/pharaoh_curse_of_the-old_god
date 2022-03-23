using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pharaoh.Gameplay
{
    public abstract class TargetFinder : MonoBehaviour
    {
        [SerializeField, Tooltip("Overlap detectable colliders"), Range(2, 20)] 
        protected int overlapCount = 5;
        [SerializeField, Tooltip("TargetFinder radius"), Range(2f, 20f)] 
        protected float overlapingRadius = 5f;
        [SerializeField, Tooltip("TargetFinder FOV"), Range(1f, 360f)] 
        protected float overlapingFov = 270f;

        [SerializeField, Tooltip("Target layers")]
        protected LayerMask whatIsTarget;
        [field: SerializeField, Tooltip("Obstacle layers")]
        public LayerMask whatIsObstacle { get; protected set; }

        protected Collider2D[] _overlaps;

        protected GameObject _bestTargetRight;
        protected GameObject _bestTargetLeft;
        protected GameObject _currentTarget;

        [SerializeField, Tooltip("Event when found the best target")]
        protected UnityEvent<TargetFinder, GameObject> onFoundBestTarget = new UnityEvent<TargetFinder, GameObject>();
        
        protected virtual void Awake()
        {
            _overlaps = new Collider2D[overlapCount];
        }

        protected void SearchTargets()
        {
            int overlapCount = Physics2D.OverlapCircleNonAlloc(transform.position, overlapingRadius, _overlaps, whatIsTarget);

            float closestDistanceRight = Mathf.Infinity;
            float closestDistanceLeft = Mathf.Infinity;

            int bestOverlapRight = -1;
            int bestOverlapLeft = -1;

            // Loops each target and remove those behind walls as well as selects the closest one
            // Selects the best target to the right of the player and to the left
            for (int overlapIndex = 0; overlapIndex < overlapCount; overlapIndex++)
            {
                var overlap = _overlaps[overlapIndex];

                if (_currentTarget == overlap.gameObject) continue;

                Vector2 direction = overlap.transform.position - transform.position;
                float distance = direction.magnitude;
                bool isOnRight = overlap.transform.position.x > transform.position.x;
                
                if (Vector2.Angle(transform.right, direction.normalized) >= overlapingFov / 2) continue;
                if (Physics2D.Raycast(transform.position, direction.normalized, distance, whatIsObstacle)) continue;

                switch (isOnRight)
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
    }

}