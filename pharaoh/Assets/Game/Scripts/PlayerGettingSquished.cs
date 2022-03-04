using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGettingSquished : MonoBehaviour
{

    private Rigidbody2D _rigidBody;

    [Tooltip("What layer is the Moving Block ?")]
    public LayerMask whatIsMovingBlock;
    [Tooltip("Transform at which the player will respawn")]
    public Transform DEBUGRespawnPoint;
    [Tooltip("Raw Image used to fade to black when respawning")]
    public RawImage blackScreen;
    [Tooltip("Duration of the fade animation")]
    public float fadeDuration;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        blackScreen.color = Color.black;
        blackScreen.canvasRenderer.SetAlpha(0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (whatIsMovingBlock == (whatIsMovingBlock | (1 << collision.gameObject.layer)) && collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < -0.1f)
        {
            // Damage player and spawn him back to spawnpoint
            blackScreen.canvasRenderer.SetAlpha(1f);
            blackScreen.CrossFadeAlpha(0f, fadeDuration, false);

            //damage player
            Debug.LogWarning("PlayerGettingSquished : need to implement the health component to damage (1) the player");
            _rigidBody.position = DEBUGRespawnPoint.position;
        }
    }

}
