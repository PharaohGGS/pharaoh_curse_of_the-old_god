using Pharaoh.GameEvents;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDamage : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
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
        _rigidBody = GetComponent<Rigidbody2D>();

        if (DEBUGRespawnPoint == null)
        {
            GameObject go = new GameObject();
            go.name = "DefaultRespawnPoint";
            go.transform.position = transform.position;
            DEBUGRespawnPoint = go.transform;
        }
    }

    public void MoveToSpawnPoint()
    {
        _rigidBody.position = DEBUGRespawnPoint.position;
    }

    public void Respawn()
    {
        if (_isRespawning) return; //the player is already respawning due to damage
        StartCoroutine(RespawnCoroutine());
    }

    private System.Collections.IEnumerator RespawnCoroutine()
    {
        _isRespawning = true;

        yield return new WaitForSeconds(delayBeforeRespawn);
        
        // Fade the screen and respawn the player
        onRespawn?.Invoke();
        _isRespawning = false;
    }

}
