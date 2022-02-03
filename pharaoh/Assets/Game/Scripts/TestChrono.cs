using UnityEngine;

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
