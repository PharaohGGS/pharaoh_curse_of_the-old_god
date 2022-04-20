using Pharaoh.Gameplay.Components.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager = Pharaoh.Managers.AudioManager;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    [Header("Audio source parameters")]
    [SerializeField]
    private float audioSourceVolume;

    [Space(10)]
    [Header("Player sounds")]
    [SerializeField]
    private AudioClip[] footStepClips;
    [SerializeField]
    private AudioClip[] jumpClips;

    [Space(10)]
    [Header("Khepesh sounds")]
    [SerializeField]
    private AudioClip[] khepeshDrawClips;
    [SerializeField]
    private AudioClip[] khepeshSwingClips;
    [SerializeField]
    private AudioClip[] khepeshDoubleSwingClips;
    [SerializeField]
    private AudioClip[] khepeshHitClips;

    [Space(10)]
    [Header("Hook sound")]
    [SerializeField]
    private AudioClip[] hookThrowClips;

    [Space(10)]
    [Header("Doors and plates sound")]
    [SerializeField]
    private AudioClip[] doorOpensClips;
    [SerializeField]
    private AudioClip[] doorClosesClips;


    private PlayerMovement playerMovementData;
    private AudioSource audioSource;

    private void Awake()
    {
        playerMovementData = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = audioSourceVolume;
        audioSource.loop = false;
    }

    public void PlayerStepSound()
    {
        if(playerMovementData.IsGrounded && playerMovementData.IsRunning)
        {
            AudioClip footStepClip = GetRandomClip(footStepClips);
            audioSource.PlayOneShot(footStepClip);
        } 
    }

    public void PlayerHookSound()
    {
        AudioClip hookThrowClip = GetRandomClip(hookThrowClips);
        audioSource.PlayOneShot(hookThrowClip);
    }

    public void PlayerDrawsSound()
    {
        AudioClip khepeshDrawClip = GetRandomClip(khepeshDrawClips);
        audioSource.PlayOneShot(khepeshDrawClip);
    }

    public void PlayerSwingSound()
    {
        AudioClip khepeshSwingClip = GetRandomClip(khepeshSwingClips);
        audioSource.PlayOneShot(khepeshSwingClip);
    }

    public void PlayerDoubleSwingSound()
    {
        AudioClip khepeshDoubleSwingClip = GetRandomClip(khepeshDoubleSwingClips);
        audioSource.PlayOneShot(khepeshDoubleSwingClip);
    }

    public void PlayerHitSound()
    {
        AudioClip khepeshHitClip = GetRandomClip(khepeshHitClips);
        audioSource.PlayOneShot(khepeshHitClip);
    }

    public void DoorOpens()
    {
        AudioClip doorOpensClip = GetRandomClip(doorOpensClips);
        audioSource.PlayOneShot(doorOpensClip);
    }

    public void DoorCloses()
    {
        AudioClip doorClosesClip = GetRandomClip(doorClosesClips);
        audioSource.PlayOneShot(doorClosesClip);
    }


    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
