using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDamage : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private Pharaoh.Gameplay.HookTargeting _hookTargeting;
    private Rigidbody2D _rigidBody;
    private FadeToBlack _fade;
    private bool _isRespawning = false;

    [Tooltip("What layer is the Moving Block ?")]
    public LayerMask whatIsMovingBlock;
    [Tooltip("What layer is the Spike ?")]
    public LayerMask whatIsSpike;

    [Header("RespawnCoroutine")]

    [Tooltip("Transform at which the player will respawn")]
    public Transform DEBUGRespawnPoint;
    [Tooltip("Delay before which the player respawns")]
    public float delayBeforeRespawn = 0.1f;
    [Tooltip("Event invoke when the player respawns")]
    public UnityEvent onRespawn;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _hookTargeting = GetComponent<Pharaoh.Gameplay.HookTargeting>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _fade = FindObjectOfType<FadeToBlack>();

        if (DEBUGRespawnPoint == null)
        {
            GameObject go = new GameObject();
            go.name = "DefaultRespawnPoint";
            go.transform.position = transform.position;
            DEBUGRespawnPoint = go.transform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Block falling on the player
        //if (whatIsMovingBlock.HasLayer(collision.gameObject.layer) && collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < -0.1f)
        //    Respawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (whatIsSpike.HasLayer(collision.gameObject.layer))
        //    RespawnCoroutine();
    }

    public void MoveToSpawnPoint()
    {
        _rigidBody.position = DEBUGRespawnPoint.position;
    }

    public void Respawn()
    {
        if (_isRespawning) return; //the player is already respawning due to damage

        // Damage player
        Debug.Log("PlayerDamage : need to implement the health component to damage (1) the player");

        //_playerMovement.enabled = false;

        StartCoroutine(RespawnCoroutine());
    }

    private System.Collections.IEnumerator RespawnCoroutine()
    {
        _isRespawning = true;

        yield return new WaitForSeconds(delayBeforeRespawn);
        
        // Fade the screen and respawn the player
        _fade.Fade();
        //_rigidBody.position = DEBUGRespawnPoint.position;
        onRespawn?.Invoke();

        //_playerMovement.enabled = true;
        //_playerMovement.RespawnCoroutine();
        //_hookTargeting.RespawnCoroutine();

        _isRespawning = false;
    }

}
