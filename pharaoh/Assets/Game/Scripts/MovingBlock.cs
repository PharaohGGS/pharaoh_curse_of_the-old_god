using UnityEngine;

public class MovingBlock : MonoBehaviour
{

    private CircleCollider2D rightHandle;
    private CircleCollider2D leftHandle;

    public LayerMask whatIsSpike;

    private void Awake()
    {
        rightHandle = transform.Find("DEBUG - Moving Block Target Right").GetComponent<CircleCollider2D>();
        leftHandle = transform.Find("DEBUG - Moving Block Target Left").GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (whatIsSpike == (whatIsSpike | (1 << collision.gameObject.layer)))
        {
            rightHandle.enabled = false;
            leftHandle.enabled = false;
        }

        this.enabled = false;
    }

}
