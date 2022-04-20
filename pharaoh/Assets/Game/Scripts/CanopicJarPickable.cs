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
        Bird,
        Monkey,
        Dog,
        Crocodile,
        Human
    };

    public InputReader inputReader;
    public PlayerSkills playerSkills;

    public BoxCollider2D boxCollider;
    public MeshFilter meshFilter;
    public VisualEffect vfxIdle;

    public Mesh openedMesh;

    public LayerMask whatIsPlayer;
    public CanopicJar jar = CanopicJar.Bird;

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
                playerSkills.hasGrapplingHook = true;
                break;

            case CanopicJar.Dog:
                playerSkills.hasSwarmDash = true;
                break;

            case CanopicJar.Human:
                playerSkills.hasSandSoldier = true;
                break;

            default:
                break;
        }

        onPickUp?.Invoke(this);
        AudioManager.Instance.Play("CanopPickup");

        boxCollider.enabled = false;
        vfxIdle.Stop();

        meshFilter.mesh = openedMesh;
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

}
