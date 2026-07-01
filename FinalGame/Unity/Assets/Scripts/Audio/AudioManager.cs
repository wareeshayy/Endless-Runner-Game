using UnityEngine;

namespace ZillRunner.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip coinClip;
        [SerializeField] private AudioClip hitClip;
        [SerializeField] private AudioClip powerUpClip;
        [SerializeField] private AudioClip jumpClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.volume = 0.35f;
            }

            if (sfxSource == null)
                sfxSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlayCoin() => PlayOneShot(coinClip, 0.7f, 1.2f);
        public void PlayHit() => PlayOneShot(hitClip, 1f, 0.9f);
        public void PlayPowerUp() => PlayOneShot(powerUpClip, 0.85f, 1f);
        public void PlayJump() => PlayOneShot(jumpClip, 0.5f, 1.1f);

        private void PlayOneShot(AudioClip clip, float volume, float pitch)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}
