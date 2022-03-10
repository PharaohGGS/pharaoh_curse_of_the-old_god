using Pharaoh.Tools;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{

    private CircleCollider2D _rightHandle;
    private CircleCollider2D _leftHandle;
    private Transform _rightGroundCheck;
    private Transform _leftGroundCheck;
    private float _groundCheckDistance = 0.05f;

    public LayerMask whatIsSpike;
    public LayerMask whatIsGround;

    private void Awake()
    {
        _rightHandle = transform.Find("DEBUG - Moving Block Target Right").GetComponent<CircleCollider2D>();
        _leftHandle = transform.Find("DEBUG - Moving Block Target Left").GetComponent<CircleCollider2D>();
        _rightGroundCheck = transform.Find("Right Ground Check").transform;
        _leftGroundCheck = transform.Find("Left Ground Check").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (whatIsSpike.HasLayer(collision.gameObject.layer))
        {
            _rightHandle.enabled = false;
            _leftHandle.enabled = false;
        }

        this.enabled = false;
    }

    // Returns whether or not the block is grounded
    public bool IsGrounded()
    {
        return Physics2D.Raycast(_rightGroundCheck.position, Vector2.down, _groundCheckDistance, whatIsGround)
            || Physics2D.Raycast(_leftGroundCheck.position, Vector2.down, _groundCheckDistance, whatIsGround);
    }

}
