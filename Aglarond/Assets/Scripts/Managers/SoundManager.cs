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
        /// <summary>
        /// <para>Play a background music</para>
        /// </summary>
        /// <param name="_clip">Music to play</param>
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

        /// <summary>
        /// <para>Activate or deactivates whether music is playing or not</para>
        /// </summary>
        public void ToggleMusic() {
            musicEnabled = !musicEnabled;
            UpdateMusic();
        }

        private void UpdateMusic() {
            musicSource.volume = musicVolume;

            if (musicSource.isPlaying != musicEnabled) {
                if (musicEnabled) {
                    musicSource.Play();
                } else {
                    musicSource.Stop();
                }
            }
        }

        /// <summary>
        /// <para>Updates music volume</para>
        /// </summary>
        /// <param name="_volume">Volume to set music to</param>
        public void UpdateMusicVolume(float _volume) {
            musicVolume = _volume;
            UpdateMusic();
        }
        #endregion

        #region SOUND EFFECTS
        /// <summary>
        /// <para>Activates or deactivates whether the sound effects are playing or not</para>
        /// </summary>
        public void ToggleEffects() {
            effectsEnabled = !effectsEnabled;
        }

        /// <summary>
        /// <para>Play an AudioClip once.</para>
        /// </summary>
        /// <param name="_clip">AudioClip that will be played</param>
        public void PlayEffect(AudioClip _clip) {
            if (effectsSource == null || !effectsEnabled || _clip == null) {
                return;
            }

            effectsSource.PlayOneShot(_clip);
        }

        /// <summary>
        /// <para>Update Sound Effects volume</para>
        /// </summary>
        /// <param name="_volume">Volume to set sound effects to</param>
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