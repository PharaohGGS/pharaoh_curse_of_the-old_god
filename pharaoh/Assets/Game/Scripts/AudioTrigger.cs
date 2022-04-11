using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManager = Pharaoh.Managers.AudioManager;

public class AudioTrigger : MonoBehaviour
{
    [Header("Sounds to play or stop")]
    [Tooltip("Sound to play on trigger activation")]
    public string soundToPlay;
    [Tooltip("Sound to stop on trigger activation")]
    public string soundToStop;

    [Space(10)]
    [Header("When to play or stop")]
    [Tooltip("Play sound when the player enter the trigger")]
    public bool playOnEnter;
    [Tooltip("Play sound when the player exit the trigger")]
    public bool playOnExit;
    [Tooltip("Stop sound when the player enter the trigger")]
    public bool stopOnEnter;
    [Tooltip("Stop sound when the player exit the trigger")]
    public bool stopOnExit;

    [Space(10)]
    [Header("Replay or Restop")]
    [Tooltip("Play sound on each trigger activation")]
    public bool Replay = false;
    [Tooltip("Stop sound on each trigger activation")]
    public bool Restop = false;

    [Header("Player layer")]
    [Tooltip("Player layer number")]
    [SerializeField] private int playerLayer;

    private bool playedOnEnter;
    private bool playedOnExit;
    private bool stoppedOnEnter;
    private bool stoppedOnExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerLayer == collision.gameObject.layer)
        {
            if (playOnEnter && soundToPlay != null && !playedOnEnter)
            {
                AudioManager.Instance.Play(soundToPlay);
                Debug.Log("--Playing " + soundToPlay + " on enter");
                playedOnEnter = !Replay;
            }

            if (stopOnEnter && soundToStop != null && !stoppedOnEnter)
            {
                AudioManager.Instance.Stop(soundToStop);
                Debug.Log("--Stoping " + soundToStop + " on enter");
                stoppedOnEnter = !Restop;
            }
        }
        else
        {
            Debug.Log("-- Not player");
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerLayer == collision.gameObject.layer)
        {
            if (playOnExit && soundToPlay != null && !playedOnExit)
            {
                AudioManager.Instance.Play(soundToPlay);
                Debug.Log("--Playing " + soundToPlay + " on exit");
                playedOnExit = !Replay;
            }

            if (stopOnExit && soundToStop != null && !stoppedOnExit)
            {
                AudioManager.Instance.Stop(soundToStop);
                Debug.Log("--Stoping " + soundToStop + " on exit");
                stoppedOnExit = !Replay;
            }
        }
        else
        {
            Debug.Log("-- Not player");
        }
        Debug.Log("param : " + playerLayer);
        Debug.Log(collision.gameObject.layer);
    }
}
