using UnityEngine;
using Pharaoh.Managers;
using Pharaoh.Tools;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanopicJarPickable : MonoBehaviour
{

    public enum CanopicJar : int
    {
        None,
        Bird,
        Monkey,
        Dog,
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
            case CanopicJar.Monkey:
            case CanopicJar.Crocodile:
                AudioManager.Instance.Play("LoreShort");
                break;

            case CanopicJar.Bird:
                playerSkills.hasGrapplingHook= true;
                AudioManager.Instance.Play("LoreShort");
                break;

            case CanopicJar.Dog:
                playerSkills.hasSwarmDash = true;
                AudioManager.Instance.Play("LoreShort");
                break;

            case CanopicJar.Human:
                playerSkills.hasSandSoldier= true;
                AudioManager.Instance.Play("LoreShort");
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

        int index = (int)jar * 2;
        canopicJars[index].SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.HasLayer(whatIsPlayer))
            inputReader.hookInteractPerformedEvent += OnInteract;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inputReader.hookInteractPerformedEvent -= OnInteract;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            ChangeSkin();
        }
    }
#endif

}
