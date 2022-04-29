using System;
using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Audio;

namespace Pharaoh.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            if (sounds == null) return;
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
        }

        [Space(10)]
        [Header("Player's sound")]
        [SerializeField]
        private AudioClip[] dashNormalClips;
        [SerializeField]
        private AudioClip[] dashSwarmClips;
        [SerializeField]
        private AudioClip[] khepeshHitClips;

        [Space(10)]
        [Header("Mob's sound")]
        [SerializeField]
        private AudioClip[] mobBarksClips;
        [SerializeField]
        private AudioClip[] clawSwingClips;
        [SerializeField]
        private AudioClip[] clawHitClips;
        [SerializeField]
        private AudioClip[] harpoonSwingClips;
        [SerializeField]
        private AudioClip[] harpoonThrowClips;
        [SerializeField]
        private AudioClip[] harpoonHitClips;

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
        
        [Space(10)]
        [Header("Traps' sound")]
        [SerializeField]
        private AudioClip[] arrowHitClips;
        [SerializeField]
        private AudioClip[] spikeHitClips;

        [Space(10)]
        [Header("Environmental sounds")]
        [SerializeField]
        private AudioClip[] RockFallClips;
        [SerializeField]
        private AudioClip[] RockRumbleClips;
        [SerializeField]
        private AudioClip[] SandFallLongClips;
        [SerializeField]
        private AudioClip[] SandFallShortClips;

        [Space(10)]
        [Header("Sounds parameters")]
        public GenericDictionary<Sound, AudioSource> soundSources = new GenericDictionary<Sound, AudioSource>();
        [SerializeField]
        private List<string> musicsToStop = new List<string>
        {"Menu", "Ambiance", "Fight", "Enigme", "LoreZone", "LoreShort", "Roomtone High", "Credits"};

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
            if (sounds == null) return;
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
                Debug.Log($"{s.name} audioSource is already playing and can't override.");
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
                    case "KhepeshHit":
                        audioSource.clip = GetRandomClip(khepeshHitClips);
                        break; 
                    case "MobBarks":
                        audioSource.clip = GetRandomClip(mobBarksClips);
                        break;
                    case "ClawSwing":
                        audioSource.clip = GetRandomClip(clawSwingClips);
                        break;
                    case "ClawHit":
                        audioSource.clip = GetRandomClip(clawHitClips);
                        break;
                    case "HarpoonSwing":
                        audioSource.clip = GetRandomClip(harpoonSwingClips);
                        break;
                    case "HarpoonThrow":
                        audioSource.clip = GetRandomClip(harpoonThrowClips);
                        break;
                    case "HarpoonHit":
                        audioSource.clip = GetRandomClip(harpoonHitClips);
                        break;
                    case "ArrowHit":
                        audioSource.clip = GetRandomClip(arrowHitClips);
                        break;
                    case "SpikeHit":
                        audioSource.clip = GetRandomClip(spikeHitClips);
                        break;
                    case "RockFall":
                        audioSource.clip = GetRandomClip(RockFallClips);
                        break;
                    case "RockRumble":
                        audioSource.clip = GetRandomClip(RockRumbleClips);
                        break;
                    case "SandFallLong":
                        audioSource.clip = GetRandomClip(SandFallLongClips);
                        break;
                    case "SandFallShort":
                        audioSource.clip = GetRandomClip(SandFallShortClips);
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
            if (sounds == null) return;
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
            if (sounds == null) return;
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

        public void StopAllMusic()
        {
            foreach (string music in musicsToStop)
            {
                Stop(music);
            }
        }

        private AudioClip GetRandomClip(AudioClip[] audioClips)
        {
            return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        }

        public void RandomBark(string soundName, int chanceOnTen)
        {
            if (UnityEngine.Random.Range(1, 10) <= chanceOnTen)
            {
                Play(soundName);
            }
        }
    }
}

