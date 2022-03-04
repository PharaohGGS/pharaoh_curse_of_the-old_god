using UnityEngine;

namespace Pharaoh.Gameplay
{
    public abstract class Targeting : MonoBehaviour
    {
        [SerializeField, Tooltip("Overlap detectable colliders"), Range(2, 20)] protected int overlapCount = 5;
        [SerializeField, Tooltip("Targeting radius"), Range(2f, 20f)] protected float overlapingRadius = 5f;
        [SerializeField, Tooltip("Target layers")] protected LayerMask whatIsTarget = 1 << 6;
        [SerializeField, Tooltip("Obstacle layers")] protected LayerMask whatIsObstacle = 1 << 6;

        protected Collider2D[] _overlaps;

        protected GameObject _bestTargetRight;
        protected GameObject _bestTargetLeft;
        protected GameObject _currentTarget;
        
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
                float distance = Vector2.Distance(overlap.transform.position, transform.position);
                bool isOnRight = overlap.transform.position.x > transform.position.x;

                var raycastHits = Physics2D.RaycastAll(transform.position, direction, distance, whatIsObstacle);
                if (raycastHits.Length >= 1) continue;

                if (isOnRight && distance < closestDistanceRight)
                {
                    bestOverlapRight = overlapIndex;
                    closestDistanceRight = distance;
                }
                else if (!isOnRight && distance < closestDistanceLeft)
                {
                    bestOverlapLeft = overlapIndex;
                    closestDistanceLeft = distance;
                }
            }

            // Selects the best targets if there is
            _bestTargetRight = bestOverlapRight == -1 ? null : _overlaps[bestOverlapRight].gameObject;
            _bestTargetLeft = bestOverlapLeft == -1 ? null : _overlaps[bestOverlapLeft].gameObject;
        }
    }
}