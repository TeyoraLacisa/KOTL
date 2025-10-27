using UnityEngine;
using UnityEngine.Audio;

namespace HorrorGame.Audio
{
    public class HorrorAudioManager : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup masterMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup sfxMixer;
        [SerializeField] private AudioMixerGroup ambientMixer;
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambientSource;
        
        [Header("Horror Audio Clips")]
        [SerializeField] private AudioClip[] ambientSounds;
        [SerializeField] private AudioClip[] jumpScareSounds;
        [SerializeField] private AudioClip[] footstepSounds;
        [SerializeField] private AudioClip[] doorSounds;
        [SerializeField] private AudioClip[] breathingSounds;
        
        [Header("Audio Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private float ambientVolume = 0.5f;
        
        [Header("Horror Effects")]
        [SerializeField] private bool enableRandomAmbient = true;
        [SerializeField] private float ambientInterval = 10f;
        [SerializeField] private float breathingInterval = 5f;
        
        private float lastAmbientTime;
        private float lastBreathingTime;
        private bool isPlayingBreathing = false;
        
        public static HorrorAudioManager Instance { get; private set; }
        
        void Awake()
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
        }
        
        void Start()
        {
            SetupAudioSources();
            StartAmbientAudio();
        }
        
        void Update()
        {
            if (enableRandomAmbient)
            {
                HandleRandomAmbient();
            }
        }
        
        void SetupAudioSources()
        {
            // Setup music source
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }
            musicSource.outputAudioMixerGroup = musicMixer;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            
            // Setup SFX source
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            sfxSource.outputAudioMixerGroup = sfxMixer;
            sfxSource.volume = sfxVolume;
            
            // Setup ambient source
            if (ambientSource == null)
            {
                ambientSource = gameObject.AddComponent<AudioSource>();
            }
            ambientSource.outputAudioMixerGroup = ambientMixer;
            ambientSource.volume = ambientVolume;
            ambientSource.spatialBlend = 0f; // 2D sound
        }
        
        void StartAmbientAudio()
        {
            if (ambientSounds.Length > 0)
            {
                PlayRandomAmbientSound();
            }
        }
        
        void HandleRandomAmbient()
        {
            // Random ambient sounds
            if (Time.time - lastAmbientTime > ambientInterval)
            {
                PlayRandomAmbientSound();
                lastAmbientTime = Time.time;
            }
            
            // Random breathing sounds
            if (Time.time - lastBreathingTime > breathingInterval && !isPlayingBreathing)
            {
                PlayRandomBreathingSound();
                lastBreathingTime = Time.time;
            }
        }
        
        public void PlayMusic(AudioClip clip)
        {
            if (musicSource != null && clip != null)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
        }
        
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
        
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip, volume);
            }
        }
        
        public void PlayRandomAmbientSound()
        {
            if (ambientSounds.Length > 0)
            {
                AudioClip randomClip = ambientSounds[Random.Range(0, ambientSounds.Length)];
                PlayAmbientSound(randomClip);
            }
        }
        
        public void PlayAmbientSound(AudioClip clip)
        {
            if (ambientSource != null && clip != null)
            {
                ambientSource.PlayOneShot(clip);
            }
        }
        
        public void PlayJumpScareSound()
        {
            if (jumpScareSounds.Length > 0)
            {
                AudioClip jumpScareClip = jumpScareSounds[Random.Range(0, jumpScareSounds.Length)];
                PlaySFX(jumpScareClip, 1.2f);
            }
        }
        
        public void PlayFootstepSound()
        {
            if (footstepSounds.Length > 0)
            {
                AudioClip footstepClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
                PlaySFX(footstepClip, 0.8f);
            }
        }
        
        public void PlayDoorSound()
        {
            if (doorSounds.Length > 0)
            {
                AudioClip doorClip = doorSounds[Random.Range(0, doorSounds.Length)];
                PlaySFX(doorClip, 0.9f);
            }
        }
        
        public void PlayRandomBreathingSound()
        {
            if (breathingSounds.Length > 0 && !isPlayingBreathing)
            {
                AudioClip breathingClip = breathingSounds[Random.Range(0, breathingSounds.Length)];
                PlaySFX(breathingClip, 0.6f);
                isPlayingBreathing = true;
                Invoke(nameof(ResetBreathingFlag), breathingClip.length);
            }
        }
        
        void ResetBreathingFlag()
        {
            isPlayingBreathing = false;
        }
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            if (masterMixer != null)
            {
                masterMixer.audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            }
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicMixer != null)
            {
                musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            }
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxMixer != null)
            {
                sfxMixer.audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
            }
        }
        
        public void SetAmbientVolume(float volume)
        {
            ambientVolume = Mathf.Clamp01(volume);
            if (ambientMixer != null)
            {
                ambientMixer.audioMixer.SetFloat("AmbientVolume", Mathf.Log10(ambientVolume) * 20);
            }
        }
        
        public void FadeOutMusic(float duration)
        {
            StartCoroutine(FadeMusicCoroutine(0f, duration));
        }
        
        public void FadeInMusic(float duration)
        {
            StartCoroutine(FadeMusicCoroutine(musicVolume, duration));
        }
        
        private System.Collections.IEnumerator FadeMusicCoroutine(float targetVolume, float duration)
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                yield return null;
            }
            
            musicSource.volume = targetVolume;
        }
    }
}