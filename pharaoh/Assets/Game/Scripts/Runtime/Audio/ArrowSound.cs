using Pharaoh.Gameplay.Components.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager = Pharaoh.Managers.AudioManager;

//[RequireComponent(typeof(AudioSource))]
public class ArrowSound : MonoBehaviour
{
    [Header("Audio source parameters")]
    [SerializeField]
    private float audioSourceVolume;
    [SerializeField]
    private float audioSourceVolumeHigh;

    [Space(10)]
    [Header("Traps sounds")]
    [SerializeField]
    private AudioClip[] arrowActivationClips;
    [SerializeField]
    private AudioClip[] spikeActivationClips;

    private AudioSource audioSource;
    private AudioSource audioSourceHigh;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();

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

    public void ArrowActivationSound()
    {
        Debug.Log("Arrow activation");
        AudioClip arrowActivationClip = GetRandomClip(arrowActivationClips);
        audioSource.PlayOneShot(arrowActivationClip);
    }

    public void SpikeActivationSound()
    {
        Debug.Log("Spike activation");
        AudioClip spikeActivationClip = GetRandomClip(spikeActivationClips);
        audioSource.PlayOneShot(spikeActivationClip);
    }
    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
