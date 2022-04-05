using UnityEngine;
using Pharaoh.Tools;

public class CanopicJarPickable : MonoBehaviour
{

    public enum Skill
    {
        None,
        SwarmDash,
        SandSoldier,
        Hook
    };

    public InputReader inputReader;
    public PlayerSkills playerSkills;

    public GameObject canopicJar;
    public GameObject canopicJarUsed;

    public BoxCollider2D boxCollider;
    public ParticleSystem psIdle;
    public ParticleSystem psPickup;

    public LayerMask whatIsPlayer;
    public Skill skill;

    public void OnInteract()
    {
        //Grant the player the given power
        switch (skill)
        {
            case Skill.SwarmDash:
                playerSkills.hasSwarmDash = true;
                break;

            case Skill.SandSoldier:
                playerSkills.hasSandSoldier = true;
                break;

            case Skill.Hook:
                playerSkills.hasGrapplingHook = true;
                break;

            default:
            case Skill.None:
                break;
        }

        boxCollider.enabled = false;
        psIdle.Stop();
        psPickup.Play();

        canopicJarUsed.SetActive(true);
        canopicJar.SetActive(false);
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

}
