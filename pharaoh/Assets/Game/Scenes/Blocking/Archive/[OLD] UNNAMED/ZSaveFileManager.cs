using UnityEngine;
using Pharaoh.Tools;
using System.Collections;

public class ZSaveFileManager : MonoBehaviour
{

    private AudioSource _fileSound;

    public InputReader inputReader;

    public GameObject saveFile;
    public ParticleSystem fileLoader;
    public AudioClip fileClip;

    public LayerMask whatIsPlayer;

    private void OnEnable()
    {
        _fileSound = gameObject.AddComponent<AudioSource>();
        _fileSound.loop = false;
        _fileSound.playOnAwake = false;
        _fileSound.volume = 0.1f;
        _fileSound.clip = fileClip;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.HasLayer(whatIsPlayer))
            inputReader.interactPerformedEvent += OnInteract;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inputReader.interactPerformedEvent -= OnInteract;
    }

    private void OnDisable()
    {
        inputReader.interactPerformedEvent -= OnInteract;
    }

    private void OnInteract()
    {
        StopAllCoroutines();
        saveFile.SetActive(true);
        fileLoader.Play();
        _fileSound.Play();
        StartCoroutine(SaveForSeconds(2f));
    }

    private IEnumerator SaveForSeconds(float s)
    {
        yield return new WaitForSeconds(s);

        saveFile.SetActive(false);
    }
}
