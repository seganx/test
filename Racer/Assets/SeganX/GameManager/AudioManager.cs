using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] musics = null;

        private WaitForEndOfFrame waitForFrame = new WaitForEndOfFrame();

        private void Awake()
        {
            instance = this;
        }

        public void Play(int index, float volume, float fadeInTime, float fadeOutTime)
        {
            StopAllCoroutines();
            index = Mathf.Clamp(index, 0, musics.Length - 1);
            StartCoroutine(DoPlay(index, volume, fadeInTime, fadeOutTime));
        }

        private IEnumerator DoPlay(int index, float volume, float fadeInTime, float fadeOutTime)
        {
            if (currentSource != null)
                StartCoroutine(DoFadeOut(currentSource, fadeOutTime));

            currentSource = gameObject.AddComponent<AudioSource>();
            currentSource.ignoreListenerVolume = true;
            currentSource.clip = musics[index];
            currentSource.loop = true;
            currentSource.volume = 0;
            currentSource.Play();

            var targetMusic = volume * MusicVolume * 0.01f;
            while (currentSource.volume < targetMusic)
            {
                currentSource.volume = Mathf.MoveTowards(currentSource.volume, targetMusic, Time.deltaTime / fadeInTime);
                yield return waitForFrame;
            }
        }

        private IEnumerator DoFadeOut(AudioSource source, float fadeOutTime)
        {
            while (source.volume > 0)
            {
                source.volume = Mathf.MoveTowards(source.volume, 0, Time.deltaTime / fadeOutTime);
                yield return waitForFrame;
            }
            Destroy(source);
        }

        //////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////////////
        private static AudioManager instance = null;
        private static AudioSource currentSource = null;
        private static int lastRandomIndex = -1;

        public static int MusicVolume
        {
            get { return PlayerPrefs.GetInt("GameSettings.MusicVolume", 100); }
            set
            {
                if (currentSource != null) currentSource.volume = value * 0.01f;
                PlayerPrefs.SetInt("GameSettings.MusicVolume", value);
            }
        }

        public static int SoundVolume
        {
            get { return PlayerPrefs.GetInt("GameSettings.SoundVolume", 100); }
            set
            {
                AudioListener.volume = value * 0.01f;
                PlayerPrefs.SetInt("GameSettings.SoundVolume", value);
            }
        }

        public static void PlayMusic(int index, float volume = 1, float fadeInTime = 1, float fadeOutTime = 1)
        {
            instance.Play(index, volume, fadeInTime, fadeOutTime);
        }

        public static void PlayRandom(int fromIndex, int ToIndex, float volume = 0.2f, float fadeInTime = 1, float fadeOutTime = 1)
        {
            var index = Random.Range(fromIndex, ToIndex + 1);
            while (index == lastRandomIndex)
                index = Random.Range(fromIndex, ToIndex + 1);
            lastRandomIndex = index;
            instance.Play(index, volume, fadeInTime, fadeOutTime);
        }
    }
}
