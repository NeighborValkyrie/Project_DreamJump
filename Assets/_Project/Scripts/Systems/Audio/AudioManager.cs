using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Systems.Audio
{
    /// <summary>
    /// BGM 및 SFX 관리 시스템
    /// v7.0: 개별 볼륨 제어 추가
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;

            [Range(0f, 1f)]
            public float volume = 1f;

            [Range(0.1f, 3f)]
            public float pitch = 1f;

            public bool loop = false;

            [HideInInspector]
            public AudioSource source;
        }

        [Header("Sounds")]
        public Sound[] sounds;

        [Header("Volume Control")]
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

        private Dictionary<string, Sound> soundDictionary;
        private List<AudioSource> musicSources = new List<AudioSource>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSounds();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSounds()
        {
            soundDictionary = new Dictionary<string, Sound>();

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;

                soundDictionary.Add(s.name, s);

                if (s.loop)
                {
                    musicSources.Add(s.source);
                }
            }

            Debug.Log($"[AudioManager] {sounds.Length}개 사운드 초기화 완료");
        }

        public void Play(string name)
        {
            if (!soundDictionary.ContainsKey(name))
            {
                Debug.LogWarning($"[AudioManager] 사운드 '{name}'를 찾을 수 없습니다.");
                return;
            }

            Sound s = soundDictionary[name];

            if (s.loop)
            {
                // BGM 재생
                if (!s.source.isPlaying)
                {
                    s.source.volume = s.volume * musicVolume;
                    s.source.Play();
                }
            }
            else
            {
                // SFX 재생 (중첩 가능)
                s.source.volume = s.volume * sfxVolume;
                s.source.PlayOneShot(s.source.clip);
            }
        }

        public void Stop(string name)
        {
            if (!soundDictionary.ContainsKey(name))
            {
                Debug.LogWarning($"[AudioManager] 사운드 '{name}'를 찾을 수 없습니다.");
                return;
            }

            soundDictionary[name].source.Stop();
        }

        public void StopAllMusic()
        {
            foreach (AudioSource source in musicSources)
            {
                source.Stop();
            }
        }

        // 볼륨 제어
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);

            foreach (AudioSource source in musicSources)
            {
                source.volume = musicVolume;
            }
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }

        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
    }
}