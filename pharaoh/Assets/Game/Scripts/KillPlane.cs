using UnityEngine;

/*
 * Used to respawn the player back into the playground if he fell
 */
public class KillPlane : MonoBehaviour
{

    public Transform spawnPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Rigidbody2D>().position = spawnPosition.position;
    }

}
