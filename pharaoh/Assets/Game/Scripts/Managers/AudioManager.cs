using System;
using System.Collections;
using UnityEngine;

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

            foreach (Sound s in sounds)
            {
                s.audioSource = gameObject.AddComponent<AudioSource>();
                s.audioSource.clip = s.clip;
                s.audioSource.volume = s.volume;
                s.audioSource.pitch = s.pitch;
                s.audioSource.loop = s.loop;
            }
        }

        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(.1f, 3f)] public float pitch = 1f;
            public bool loop = false;
            public bool fadeIn = false;
            public bool fadeOut = false;
            public float fadeDuration = 1f;
            [HideInInspector] public AudioSource audioSource;
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

            s.audioSource?.Play();
            if (s.fadeIn)
            {
                StartCoroutine(StartFadeIn(s.audioSource, 5, s.volume));
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

            s.audioSource?.Pause();
        }

        public void Stop(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogWarning("Sound " + name + " not found !");
                return;
            }

            
            if (s.fadeOut)
            {
                StartCoroutine(StartFadeIn(s.audioSource, 5, s.volume));
            } 
            else
            {
                s.audioSource?.Stop();
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

        public static IEnumerator StartFadeOut(Sound s, float duration)
        {
            float currentTime = 0;
            float start = s.audioSource.volume;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                s.audioSource.volume = Mathf.Lerp(start, 0, currentTime / duration);
                yield return null;
            }
            s.audioSource.Stop();
            yield break;
        }
    }
}

