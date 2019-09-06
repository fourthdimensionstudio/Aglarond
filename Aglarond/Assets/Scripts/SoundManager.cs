using UnityEngine;

namespace FourthDimension.Managers {
    public class SoundManager : MonoBehaviour {
        public static SoundManager instance;

        [Header("Music")]
        public AudioSource musicSource;
        public bool musicEnabled = true;
        [Range(0, 1)]
        public float musicVolume = 1.0f;

        [Header("Sound Effects")]
        public AudioSource effectsSource;
        public bool effectsEnabled = true;
        [Range(0, 1)]
        public float effectsVolume = 1.0f;

        private void Awake() {
            if(instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        private void Start() {
            musicSource.volume = musicVolume;
            effectsSource.volume = effectsVolume;
        }

        #region MUSIC
        public void PlayBackgroundMusic(AudioClip _clip) {
            if (!musicEnabled || !musicSource || !_clip) {
                return;
            }

            musicSource.Stop();
            musicSource.clip = _clip;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void ToggleMusic() {
            musicEnabled = !musicEnabled;
            UpdateMusic();
        }

        public void UpdateMusic() {
            musicSource.volume = musicVolume;

            if (musicSource.isPlaying != musicEnabled) {
                if (musicEnabled) {
                    musicSource.Play();
                } else {
                    musicSource.Stop();
                }
            }
        }

        public void UpdateMusicVolume(float _volume) {
            musicVolume = _volume;
            UpdateMusic();
        }
        #endregion

        #region SOUND EFFECTS
        public void ToggleEffects() {
            effectsEnabled = !effectsEnabled;
        }

        /// <summary>
        /// Play an AudioClip once.
        /// </summary>
        /// <param name="_clip">AudioClip that will be played</param>
        public void PlayEffect(AudioClip _clip) {
            if (effectsSource == null || !effectsEnabled || _clip == null) {
                return;
            }

            effectsSource.PlayOneShot(_clip);
        }

        public void UpdateEffectsVolume(float _volume) {
            effectsVolume = _volume;
            UpdateEffects();
        }

        private void UpdateEffects() {
            effectsSource.volume = effectsVolume;
        }
        #endregion
    }
}