using UnityEngine;
using Pharaoh.Tools;
using UnityEditor;

public class CanopicJarPickable : MonoBehaviour
{

    public enum CanopicJar
    {
        None,
        Bird,
        Monkey,
        Crocodile,
        Human
    };

    public InputReader inputReader;
    public PlayerSkills playerSkills;

    public BoxCollider2D boxCollider;
    public ParticleSystem psIdle;
    public ParticleSystem psPickup;

    public GameObject[] canopicJars;

    public LayerMask whatIsPlayer;
    public CanopicJar jar = CanopicJar.None;

    private void Awake()
    {
        //ChangeSkin();
    }

    public void OnInteract()
    {
        //Grant the player the given power
        switch (jar)
        {
            case CanopicJar.Bird:
                playerSkills.hasSwarmDash = true;
                break;

            case CanopicJar.Monkey:
                playerSkills.hasSandSoldier = true;
                break;

            case CanopicJar.Crocodile:
                playerSkills.hasGrapplingHook = true;
                break;

            default:
                break;
        }

        boxCollider.enabled = false;
        psIdle.Stop();
        psPickup.Play();

        canopicJars[(int)jar * 2].SetActive(false);
        canopicJars[((int)jar * 2) + 1].SetActive(true);
    }

    private void ChangeSkin()
    {
        foreach (GameObject go in canopicJars) go.SetActive(false);
        canopicJars[(int)jar * 2].SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (whatIsPlayer == (whatIsPlayer | 1 << collision.gameObject.layer))
            inputReader.hookInteractPerformedEvent += OnInteract;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inputReader.hookInteractPerformedEvent -= OnInteract;
    }

    private void OnValidate()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            ChangeSkin();
    }

}
