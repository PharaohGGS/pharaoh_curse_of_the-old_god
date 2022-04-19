using Pharaoh.Gameplay.Components.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager = Pharaoh.Managers.AudioManager;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    [SerializeField]
    private float audioSourceVolume;
    [SerializeField]
    private AudioClip[] footStepClips;
    [SerializeField]
    private AudioClip[] khepeshSwingClips;
    [SerializeField]
    private AudioClip[] jumpClips;


    private PlayerMovement playerMovementData;
    private AudioSource audioSource;

    private void Awake()
    {
        playerMovementData = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = audioSourceVolume;
        audioSource.loop = false;
    }

    public void PlayerStep()
    {
        if(playerMovementData.IsGrounded && playerMovementData.IsRunning)
        {
            AudioClip footStepClip = GetRandomClip(footStepClips);
            audioSource.PlayOneShot(footStepClip);
        } 
    }

    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
