using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGettingSquished : MonoBehaviour
{

    private Rigidbody2D _rigidBody;
    private FadeToBlack _fade;

    [Tooltip("What layer is the Moving Block ?")]
    public LayerMask whatIsMovingBlock;
    [Tooltip("Transform at which the player will respawn")]
    public Transform DEBUGRespawnPoint;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _fade = FindObjectOfType<FadeToBlack>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (whatIsMovingBlock == (whatIsMovingBlock | (1 << collision.gameObject.layer)) && collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < -0.1f)
        {
            // Damage player and spawn him back to spawnpoint
            _fade.Fade();

            //damage player
            Debug.LogWarning("PlayerGettingSquished : need to implement the health component to damage (1) the player");
            _rigidBody.position = DEBUGRespawnPoint.position;
        }
    }

}
