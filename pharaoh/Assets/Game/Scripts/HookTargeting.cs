using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerMovement))]
public class HookTargeting : MonoBehaviour
{

    private Rigidbody2D _rigidbody;
    private PlayerMovement _playerMovement;
    private GameObject _bestTargetRight;
    private GameObject _bestTargetLeft;

    [Tooltip("Target layers")]
    public LayerMask whatIsTarget;
    [Tooltip("Wall layers")]
    public LayerMask whatIsWall;
    [Tooltip("Targeting radius")]
    public float targetingRadius = 6f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Look for targets in targeting range
        Collider2D[] hits = Physics2D.OverlapCircleAll(_rigidbody.position, targetingRadius, whatIsTarget);

        // Loops each target and remove those behind walls as well as selects the closest one
        // Selects the best target to the right of the player and to the left
        int bestIdxRight = -1, bestIdxLeft = -1;
        float closestDistanceRight = float.MaxValue, closestDistanceLeft = float.MaxValue;
        for (int i = 0; i < hits.Length; i++)
        {
            bool isToTheRight = hits[i].transform.position.x > _rigidbody.position.x; //is the target to the right of the player ?
            Vector2 direction = ((Vector2)hits[i].transform.position - _rigidbody.position).normalized; //direction player -> target
            float distance = Vector2.Distance(_rigidbody.position, (Vector2)hits[i].transform.position); //distance player -> target
            
            if (isToTheRight
                && distance < closestDistanceRight
                && Physics2D.RaycastAll(_rigidbody.position, direction, distance, whatIsWall).Length < 1)
            {
                bestIdxRight = i;
                closestDistanceRight = distance;
            }
            else if (!isToTheRight
                && distance < closestDistanceLeft
                && Physics2D.RaycastAll(_rigidbody.position, direction, distance, whatIsWall).Length < 1)
            {
                bestIdxLeft = i;
                closestDistanceLeft = distance;
            }
        }

        // Selects the best targets if there is
        _bestTargetRight = bestIdxRight == -1 ? null : hits[bestIdxRight].gameObject;
        _bestTargetLeft = bestIdxLeft == -1 ? null : hits[bestIdxLeft].gameObject;
    }

    private void OnDrawGizmos()
    {
        if (_rigidbody == null)
            return;

        // Draws the best target to the right (red if not the faced direction)
        Gizmos.color = _playerMovement.IsFacingRight ? new Color(1f, 0.7531517f, 0f, 1f) : new Color(1f, 0.7531517f, 0f, 0.1f);
        if (_bestTargetRight != null)
            Gizmos.DrawLine(_rigidbody.position, _bestTargetRight.transform.position);

        // Draws the best target to the left (red if not the faced direction)
        Gizmos.color = !_playerMovement.IsFacingRight ? new Color(1f, 0.7531517f, 0f, 1f) : new Color(1f, 0.7531517f, 0f, 0.1f);
        if (_bestTargetLeft != null)
            Gizmos.DrawLine(_rigidbody.position, _bestTargetLeft.transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (_rigidbody == null)
            return;

        // Draws a disc around the player displaying the targeting range
        Handles.color = new Color(1f, 0.7531517f, 0f, 1f);
        Handles.DrawWireDisc(_rigidbody.position, Vector3.forward, targetingRadius);
    }

}
