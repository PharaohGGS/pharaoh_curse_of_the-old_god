using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Audio;

namespace Pharaoh.Managers
{
    public class AudioManager : PersistantMonoSingleton<AudioManager>
    {
        [Space(10)]
        [Header("Player and enemies")]
        [SerializeField]
        private AudioClip[] dashNormalClips;
        [SerializeField]
        private AudioClip[] dashSwarmClips;

        [Space(10)]
        [Header("Doors and crates and plates sound")]
        [SerializeField]
        private AudioClip[] doorOpensClips;
        [SerializeField]
        private AudioClip[] doorClosesClips;
        [SerializeField]
        private AudioClip[] plateOnClips;
        [SerializeField]
        private AudioClip[] plateOffClips;
        [SerializeField]
        private AudioClip[] cratePullClips;

        public GenericDictionary<Sound, AudioSource> soundSources = new GenericDictionary<Sound, AudioSource>();

        protected override void Awake()
        {

            foreach (Sound s in sounds)
            {
                var go = new GameObject($"{s.name} source");
                go.transform.SetParent(transform);
                var source = go.AddComponent<AudioSource>();

                source.outputAudioMixerGroup = s.audioMixerGroup;
                source.clip = s.clip;
                source.volume = s.volume;
                source.pitch = s.pitch;
                source.loop = s.loop;

                if (!soundSources.TryAdd(s, source))
                {
                    Debug.LogError($"Can't add source to dico");
                }
            }

            base.Awake();
        }

        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(.1f, 3f)] public float pitch = 1f;
            public AudioMixerGroup audioMixerGroup;
            public bool loop = false;
            public bool fadeIn = false;
            public bool fadeOut = false;
            public float fadeInDuration = 1f;
            public float fadeOutDuration = 1f;
            public bool randomized = false;
            public bool canOverride = false;
        }

        public Sound[] sounds;

        public void Play(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            
            if (s == null)
            {
                Debug.LogWarning("Sound " + name + " not found !");
                return;
            }

            if (!soundSources.TryGetValue(s, out AudioSource audioSource))
            {
                Debug.LogWarning($"Sound {name} doesn't have audioSource !");
                return;
            }

            if (audioSource.isPlaying && !s.canOverride)
            {
                Debug.LogWarning($"{s.name} audioSource is already playing and can't override.");
                return;
            }

            if (s.randomized)
            {
                switch (name)
                {
                    case "DoorOpens":
                        audioSource.clip = GetRandomClip(doorOpensClips);
                        break;
                    case "DoorCloses":
                        audioSource.clip = GetRandomClip(doorClosesClips);
                        break;
                    case "PlateOn":
                        audioSource.clip = GetRandomClip(plateOnClips);
                        break;
                    case "PlateOff":
                        audioSource.clip = GetRandomClip(plateOffClips);
                        break;
                    case "CratePull":
                        audioSource.clip = GetRandomClip(cratePullClips);
                        break;
                    case "DashNormal":
                        audioSource.clip = GetRandomClip(dashNormalClips);
                        break;
                    case "DashSwarm":
                        audioSource.clip = GetRandomClip(dashSwarmClips);
                        break;
                    default :
                        Debug.LogWarning("Random sound " + name + " not found !");
                        break;
                }
            }

            if (audioSource.clip) audioSource.Play();
            else if (!audioSource.clip && s.clip) audioSource.PlayOneShot(s.clip);
            else Debug.LogWarning($"{s.name} doesn't have audioClip");

            if (s.fadeIn)
            {
                StartCoroutine(StartFadeIn(audioSource, s.fadeInDuration, s.volume));
            }
        }

        public void Pause(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogWarning("Sound " + name + " not found !");
                return;
            }

            if (!soundSources.TryGetValue(s, out AudioSource audioSource))
            {
                Debug.LogWarning($"Sound {name} doesn't have audioSource !");
            }

            audioSource?.Pause();
        }

        public void Stop(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogWarning("Sound " + name + " not found !");
                return;
            }
            
            if (!soundSources.TryGetValue(s, out AudioSource audioSource))
            {
                Debug.LogWarning($"Sound {name} doesn't have audioSource !");
            }
            
            if (s.fadeOut)
            {
                StartCoroutine(StartFadeOut(audioSource, s.fadeOutDuration));
            } 
            else
            {
                audioSource?.Stop();
            }
        }

        public static IEnumerator StartFadeIn(AudioSource s, float duration, float targetVolume)
        {
            float currentTime = 0;
            s.volume = 0;
            float start = s.volume;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                s.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            yield break;
        }

        public static IEnumerator StartFadeOut(AudioSource s, float duration)
        {
            float currentTime = 0;
            float start = s.volume;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                s.volume = Mathf.Lerp(start, 0, currentTime / duration);
                yield return null;
            }
            s.Stop();
            yield break;
        }

        private AudioClip GetRandomClip(AudioClip[] audioClips)
        {
            return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        }
    }
}

