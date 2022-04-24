using UnityEngine;
using UnityEngine.Events;
using Pharaoh.Managers;
using Pharaoh.Tools;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanopicJarPickable : MonoBehaviour
{

    public enum CanopicJar : int
    {
        Monkey,     // Dash
        Bird,       // Bandelettes
        Dog,        // Dash Nuée
        Human,      // Soldat de sable
        Crocodile   // Rien (coeur)
    };

    public InputReader inputReader;
    public PlayerSkills playerSkills;

    public BoxCollider2D boxCollider;
    public MeshFilter meshFilter;
    public VisualEffect vfxIdle;

    public Mesh openedMesh;

    public LayerMask whatIsPlayer;
    public CanopicJar jar = CanopicJar.Monkey;

    public UnityEvent<CanopicJarPickable> onPickUp;

    public void OnInteract()
    {
        //Grant the player the given power
        switch (jar)
        {
            case CanopicJar.Monkey:
                playerSkills.HasDash = true;
                break;

            case CanopicJar.Bird:
                playerSkills.HasGrapplingHook = true;
                break;

            case CanopicJar.Dog:
                playerSkills.HasSwarmDash = true;
                break;

            case CanopicJar.Human:
                playerSkills.HasSandSoldier = true;
                break;

            case CanopicJar.Crocodile:
                playerSkills.HasHeart = true;
                break;

            default:
                break;
        }

        AudioManager.Instance.Play("CanopPickup");

        Open();
    }

    public void Open()
    {
        onPickUp?.Invoke(this);

        boxCollider.enabled = false;
        vfxIdle.Stop();
        meshFilter.mesh = openedMesh;
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

    public void PlayCredits()
    {
        SceneManager.LoadScene("Credits");
    }

}
