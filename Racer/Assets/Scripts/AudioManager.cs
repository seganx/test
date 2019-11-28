using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Base
{
    #region fields
    [SerializeField] private AudioSource musicAudioSource = null;
    [SerializeField] private MusicClipData[] myMusicClipDatasArr = null;

    private bool isFadingOut = false;
    private float musicMaxVolume = 0.5f;
    private readonly float maxFxVolume = 0.75f;

    public static AudioManager Instance { get; private set; }
    #endregion

    #region properties
    private readonly string isMusicOnString = "IsMusicOn";

    public bool IsMusicOn
    {
        get { return PlayerPrefs.GetInt(isMusicOnString, 1) == 1; }
        set
        {
            musicAudioSource.mute = !value;
            PlayerPrefs.SetInt(isMusicOnString, value ? 1 : 0);
        }
    }

    private readonly string isFxOnString = "IsFxOn";
    public bool IsFxOn
    {
        get { return PlayerPrefs.GetInt(isFxOnString, 1) == 1; }
        set
        {
            AudioListener.volume = value ? maxFxVolume : 0;
            PlayerPrefs.SetInt(isFxOnString, value ? 1 : 0);
        }
    }
    #endregion

    #region methods
    private void Awake()
    {
        Instance = this;

        musicAudioSource.ignoreListenerPause = true;
        musicAudioSource.ignoreListenerVolume = true;
        musicMaxVolume = musicAudioSource.volume;
        musicAudioSource.volume = 0;

        if (!IsMusicOn)
            musicAudioSource.mute = true;
        AudioListener.volume = IsFxOn ? maxFxVolume : 0;
    }

    public void PlaySceneMusic(int sceneIndex, float fadeDuration)
    {
        //if (!IsMusicOn)
        //  return;

        int newClipIndex = UnityEngine.Random.Range(0, myMusicClipDatasArr[sceneIndex].MusicClipsArr.Length);

        AudioClip clip = myMusicClipDatasArr[sceneIndex].MusicClipsArr[newClipIndex];

        if (musicAudioSource.isPlaying)
        {
            isFadingOut = true;
            StartCoroutine(DoFadeOut(fadeDuration / 2, () =>
            {
                PlayMusic(clip);
                StartCoroutine(DoFadeIn(fadeDuration / 2));
            }));
        }
        else
        {
            PlayMusic(clip);
            StartCoroutine(DoFadeIn(fadeDuration / 2));
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public void StopMusic(float fadeDuration, Action _onStopFinish = null)
    {
        StartCoroutine(DoFadeOut(fadeDuration, () =>
        {
            if (_onStopFinish != null)
                _onStopFinish();
        }));
    }

    private IEnumerator DoFadeIn(float fadeInDuration)
    {
        if (fadeInDuration <= 0)
            musicAudioSource.volume = musicMaxVolume;

        while (musicAudioSource.volume < musicMaxVolume && !isFadingOut)
        {
            musicAudioSource.volume += musicMaxVolume * Time.deltaTime / fadeInDuration;
            musicAudioSource.volume = Mathf.Min(musicAudioSource.volume, musicMaxVolume);
            yield return null;
        }
    }

    private int fadeOutCounter;

    private IEnumerator DoFadeOut(float fadeOutDuration, Action onFinished)
    {
        isFadingOut = true;
        int counter = ++fadeOutCounter;
        while (musicAudioSource.volume > 0)
        {
            musicAudioSource.volume -= musicMaxVolume * Time.deltaTime / fadeOutDuration;
            yield return null;
        }
        if (counter == fadeOutCounter)
        {
            isFadingOut = false;
            if (onFinished != null)
                onFinished();
        }
    }

    public void MuteMusicForDuration(float pauseDuration, float fadeOutDuration, float fadeInDuration)
    {
        StartCoroutine(DoFadeOut(fadeOutDuration, null));
        DelayCall(pauseDuration, () =>
        {
            StartCoroutine(DoFadeIn(fadeInDuration));
        });
    }
    #endregion
}

[Serializable]
public class MusicClipData
{
    public AudioClip[] MusicClipsArr;
}