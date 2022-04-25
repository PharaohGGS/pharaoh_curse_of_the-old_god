using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager = Pharaoh.Managers.AudioManager;

public class AudioTrigger : MonoBehaviour
{
    [Header("Play a sound on trigger activation")]
    [Tooltip("Sound to play on trigger activation")]
    public string soundToPlay;
    [Space(5)]
    [Tooltip("Play sound when the player enter the trigger")]
    public bool playOnEnter;
    [Tooltip("Play sound when the player exit the trigger")]
    public bool playOnExit;
    [Space(5)]
    [Tooltip("Play sound on each trigger activation")]
    public bool Replay = false;

    [Space(10)]

    [Header("Stop a sound on trigger activation")]
    [Tooltip("Sound to stop on trigger activation")]
    public string soundToStop;
    [Space(5)]
    [Tooltip("Stop sound when the player enter the trigger")]
    public bool stopOnEnter;
    [Tooltip("Stop sound when the player exit the trigger")]
    public bool stopOnExit;
    [Space(5)]
    [Tooltip("Stop sound on each trigger activation")]
    public bool Restop = false;

    [Header("Player layer")]
    [Tooltip("Player layer number")]
    [SerializeField] private int playerLayer;

    private bool playedOnEnter = false;
    private bool playedOnExit = false;
    private bool stoppedOnEnter = false;
    private bool stoppedOnExit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerLayer == collision.gameObject.layer)
        {
            if (playOnEnter && soundToPlay != null && !playedOnEnter)
            {
                AudioManager.Instance?.Play(soundToPlay);
                playedOnEnter = !Replay;
            }

            if (stopOnEnter && soundToStop != null && !stoppedOnEnter)
            {
                AudioManager.Instance?.Stop(soundToStop);
                stoppedOnEnter = !Restop;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerLayer == collision.gameObject.layer)
        {
            if (playOnExit && soundToPlay != null && !playedOnExit)
            {
                AudioManager.Instance?.Play(soundToPlay);
                playedOnExit = !Replay;
            }

            if (stopOnExit && soundToStop != null && !stoppedOnExit)
            {
                AudioManager.Instance?.Stop(soundToStop);
                stoppedOnExit = !Restop;
            }
        }
    }
}
