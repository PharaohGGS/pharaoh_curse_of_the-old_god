using UnityEngine;

/*
 * Used to measure the time it took for the player to cross D�part and then Arriv�e
 * It really is a debug thingy
 */
public class TestChrono : MonoBehaviour
{

    private float time;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Trigger D�part")
        {
            time = Time.time;
        }
        else if (collision.gameObject.name == "Trigger Arriv�e")
        {
            Debug.Log("Travelled 10 meters in " + (Time.time - time) + " seconds !");
        }
    }

}
