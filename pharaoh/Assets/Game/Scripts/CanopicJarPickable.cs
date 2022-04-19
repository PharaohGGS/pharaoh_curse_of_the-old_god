using UnityEngine;
using UnityEngine.Events;
using Pharaoh.Managers;
using Pharaoh.Tools;
using UnityEngine.VFX;
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
    public VisualEffect vfxIdle;

    public GameObject[] canopicJars;

    public LayerMask whatIsPlayer;
    public CanopicJar jar = CanopicJar.None;

    public UnityEvent<CanopicJarPickable> onPickUp;

    public void OnInteract()
    {
        //Grant the player the given power
        switch (jar)
        {
            case CanopicJar.Monkey:
            case CanopicJar.Crocodile:
                break;

            case CanopicJar.Bird:
                playerSkills.hasGrapplingHook= true;
                break;

            case CanopicJar.Dog:
                playerSkills.hasSwarmDash = true;
                break;

            case CanopicJar.Human:
                playerSkills.hasSandSoldier= true;
                break;

            default:
                break;
        }

        onPickUp?.Invoke(this);
        AudioManager.Instance.Play("CanopPickup");

        boxCollider.enabled = false;
        vfxIdle.Stop();

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
