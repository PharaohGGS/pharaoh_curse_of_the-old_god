using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager = Pharaoh.Managers.AudioManager;
using Pharaoh.Tools;

public class AudioTrigger : MonoBehaviour
{
    [Header("Play a sound on trigger activation")]
    [Tooltip("Sound to play on trigger activation")]
    public List<string> soundsToPlay;
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
    public List<string> soundsToStop;
    [Space(5)]
    [Tooltip("Stop sound when the player enter the trigger")]
    public bool stopOnEnter;
    [Tooltip("Stop sound when the player exit the trigger")]
    public bool stopOnExit;
    [Space(5)]
    [Tooltip("Stop sound on each trigger activation")]
    public bool Restop = false;
    [Tooltip("Stop all other music than the one to play")]
    public bool StopAllMusic = false;

    [Header("Player layer")]
    [Tooltip("Player layer number")]
    [SerializeField] private LayerMask playerLayers;

    private bool playedOnEnter = false;
    private bool playedOnExit = false;
    private bool stoppedOnEnter = false;
    private bool stoppedOnExit = false;

    private List<string> musicList = new List<string>
        {"Ambiance", "LoreZone", "LoreShort", "Fight"};

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.HasLayer(playerLayers))
        {
            if (playOnEnter && soundsToPlay.Count > 0 && !playedOnEnter)
            {
                foreach(string soundToPlay in soundsToPlay)
                {
                    AudioManager.Instance?.Play(soundToPlay);
                    playedOnEnter = !Replay;
                }
            }

            if (stopOnEnter && !stoppedOnEnter)
            {
                if (StopAllMusic)
                {
                    var musicToStop = musicList;
                    musicToStop.RemoveAll(IsMusicToPlay);
                    Debug.Log(musicToStop);
                    foreach (string music in musicToStop)
                    {
                        AudioManager.Instance?.Stop(music);
                    }
                } 
                else if (soundsToStop.Count > 0)
                {
                    foreach (string soundToStop in soundsToStop)
                    {
                        AudioManager.Instance?.Stop(soundToStop);
                        stoppedOnEnter = !Restop;
                    }
                }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.HasLayer(playerLayers))
        {
            if (playOnExit && soundsToPlay.Count > 0 && !playedOnExit)
            {
                foreach (string soundToPlay in soundsToPlay)
                {
                    AudioManager.Instance?.Play(soundToPlay);
                    playedOnExit = !Replay;
                }
            }

            if (stopOnExit && !stoppedOnExit)
            {
                if (StopAllMusic)
                {
                    var musicToStop = musicList;
                    musicToStop.RemoveAll(IsMusicToPlay);
                    Debug.Log(musicToStop);
                    foreach (string music in musicToStop)
                    {
                        AudioManager.Instance?.Stop(music);
                    }
                } 
                else if(soundsToStop.Count > 0)
                {
                    foreach (string soundToStop in soundsToStop)
                    {
                        AudioManager.Instance?.Stop(soundToStop);
                        
                    }
                }
                stoppedOnExit = !Restop;
            }
        }
    }

    private bool IsMusicToPlay(string s)
    {
        var isMusicToPlay = true;
        foreach(string music in soundsToPlay)
        {
            if(s == music)
            {
                return true;
            }
        }
        Debug.Log(isMusicToPlay);
        return false;
    }
}
