using UnityEngine;
using Pharaoh.Tools;
using System.Collections;

public class ZSaveFileManager : MonoBehaviour
{
    public InputReader inputReader;

    public GameObject saveFile;
    public ParticleSystem fileLoader;

    public LayerMask whatIsPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.HasLayer(whatIsPlayer))
            inputReader.interactPerformedEvent += OnInteract;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inputReader.interactPerformedEvent -= OnInteract;
    }

    private void OnInteract()
    {
        StopAllCoroutines();
        saveFile.SetActive(true);
        fileLoader.Play();
        StartCoroutine(SaveForSeconds(2f));
    }

    private IEnumerator SaveForSeconds(float s)
    {
        yield return new WaitForSeconds(s);

        saveFile.SetActive(false);
    }
}
