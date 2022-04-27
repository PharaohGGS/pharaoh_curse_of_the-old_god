using Pharaoh.Gameplay.Components.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager = Pharaoh.Managers.AudioManager;

[RequireComponent(typeof(AudioSource))]
public class MobSound : MonoBehaviour
{
    [Header("Audio source parameters")]
    [SerializeField]
    private float audioSourceVolume;
    [SerializeField]
    private float audioSourceVolumeHigh;

    [Space(10)]
    [Header("Mob sounds")]
    [SerializeField]
    private AudioClip[] footStepClips;
    [SerializeField]
    private AudioClip[] deathClips;

    private AudioSource audioSource;
    private AudioSource audioSourceHigh;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = audioSourceVolume;
        audioSource.loop = false;

        // Creating second audioSource for high volume sounds
        var go = new GameObject($"High audioSource");
        go.transform.SetParent(transform);
        audioSourceHigh = go.AddComponent<AudioSource>();

        audioSourceHigh.volume = audioSourceVolumeHigh;
        audioSourceHigh.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        audioSourceHigh.loop = false;
    }

    public void MobStepSound()
    {
        AudioClip footStepClip = GetRandomClip(footStepClips);
        audioSource.PlayOneShot(footStepClip);
    }

    public void MobDeathSound()
    {
        Debug.Log("----Play death mob sound");
        AudioClip deathClip = GetRandomClip(deathClips);
        audioSource.PlayOneShot(deathClip);
    }
    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
