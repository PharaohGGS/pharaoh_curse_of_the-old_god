using UnityEngine;

/*
 * Used to measure the time it took for the player to cross Départ and then Arrivée
 * It really is a debug thingy
 */
public class TestChrono : MonoBehaviour
{

    private float time;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Trigger Départ")
        {
            time = Time.time;
        }
        else if (collision.gameObject.name == "Trigger Arrivée")
        {
            Debug.Log("Travelled 10 meters in " + (Time.time - time) + " seconds !");
        }
    }

}
