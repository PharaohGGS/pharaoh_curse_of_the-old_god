using UnityEngine;
using UnityEngine.Events;
using Pharaoh.Managers;
using Pharaoh.Tools;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using System.Collections;
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

    [Header("Color transition")]
    [ColorUsage(true, true)] public Color defaultColor;
    [ColorUsage(true, true)] public Color hoverColor;
    public float transitionDuration = 0.5f;

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
        if (jar == CanopicJar.Crocodile)
        {
            AudioManager.Instance?.StopAllMusic();
            PlayCredits();
        }
    }

    public void Open()
    {
        boxCollider.enabled = false;
        vfxIdle.Stop();
        meshFilter.mesh = openedMesh;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.HasLayer(whatIsPlayer))
        {
            StopAllCoroutines();
            StartCoroutine(FadeColor(vfxIdle.GetVector4("FireColor"), hoverColor));
            inputReader.interactPerformedEvent += OnInteract;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.HasLayer(whatIsPlayer))
        {
            StopAllCoroutines();
            StartCoroutine(FadeColor(vfxIdle.GetVector4("FireColor"), defaultColor));
            inputReader.interactPerformedEvent -= OnInteract;
        }
    }

    private void OnDisable()
    {
        inputReader.interactPerformedEvent -= OnInteract;
    }

    public void PlayCredits()
    {
        AudioManager.Instance?.Play("Credits");
        SceneManager.LoadScene("Credits");
    }

    private IEnumerator FadeColor(Vector4 startColor, Vector4 endColor)
    {
        float elapsedTime = 0f;
        float duration = transitionDuration;

        while (elapsedTime < duration)
        {
            Vector4 currentColor = Vector4.Lerp(startColor, endColor, elapsedTime / duration);
            vfxIdle.SetVector4("FireColor", currentColor);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        vfxIdle.SetVector4("FireColor", endColor);
        yield return null;
    }

}
