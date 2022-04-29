using Pharaoh.Gameplay.Components.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager = Pharaoh.Managers.AudioManager;

public class TrapSound : MonoBehaviour
{
    [Header("Audio source parameters")]
    [SerializeField]
    private float audioSourceVolume;

    [Space(10)]
    [Header("Traps sounds")]
    [SerializeField]
    private AudioClip[] activationClips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = audioSourceVolume;
        audioSource.loop = false;
    }

    public void ActivationSound()
    {
        AudioClip activationClip = GetRandomClip(activationClips);
        audioSource.PlayOneShot(activationClip);
    }

    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
