using UnityEngine;

public class RunChrono : MonoBehaviour
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
            Debug.Log("Ran 10 meters in " + (Time.time - time) + " seconds !");
        }
    }

}
