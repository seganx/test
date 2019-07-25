using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : Base
{
    #region fields
    [SerializeField]
    AudioSource musicAudioSource;
    [SerializeField]
    MusicClipData[] myMusicClipDatasArr;

    bool isFadingOut = false;
    float maxVolume;
    readonly float maxFxVolume = .5f;

    public static AudioManager Instance { get; private set; }
    #endregion

    #region properties
    readonly string isMusicOnString = "IsMusicOn";

    public bool IsMusicOn
    {
        get { return PlayerPrefs.GetInt(isMusicOnString, 1) == 1; }
        set
        {
            musicAudioSource.mute = !value;
            PlayerPrefs.SetInt(isMusicOnString, value ? 1 : 0);
        }
    }

    readonly string isFxOnString = "IsFxOn";
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
    void Awake()
    {
        Instance = this;

        musicAudioSource.ignoreListenerPause = true;
        musicAudioSource.ignoreListenerVolume = true;
        maxVolume = musicAudioSource.volume;
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

    void PlayMusic(AudioClip clip)
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

    IEnumerator DoFadeIn(float fadeInDuration)
    {
        if (fadeInDuration <= 0)
            musicAudioSource.volume = maxVolume;

        while (musicAudioSource.volume < maxVolume && !isFadingOut)
        {
            musicAudioSource.volume += maxVolume * Time.deltaTime / fadeInDuration;
            musicAudioSource.volume = Mathf.Min(musicAudioSource.volume, maxVolume);
            yield return null;
        }
    }

    int fadeOutCounter;
    IEnumerator DoFadeOut(float fadeOutDuration, Action onFinished)
    {
        isFadingOut = true;
        int counter = ++fadeOutCounter;
        while (musicAudioSource.volume > 0)
        {
            musicAudioSource.volume -= maxVolume * Time.deltaTime / fadeOutDuration;
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