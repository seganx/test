using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerAudio : Base
{
    [SerializeField]
    AudioClip engineIdleAudioClip = null, engineSpeedUpAudioClip = null, windAudioClip = null, sandRollingAudioClip = null, asphaltRollingAudioClip = null, asphaltSkidAudioClip = null, nosStartAudioClip = null, nosLoopAudioClip = null, hornAudioClip = null, turboIdleAudioClip = null, turboSpeedAudioClip = null;
    AudioSource engineIdleAudioSource, engineSpeedUpAudioSource, windAudioSource, sandRollingAudioSource, asphaltRollingAudioSource, asphaltSkidAudioSource, nosStartAudioSrouce, nosLoopAudioSource, hornAudioSource, turboIdleAudioSource, turboSpeedAudioSource;
    Vector3 curSpeed;
    const float AUDIO_RATIO = .7f;
    Transform mpTr;
    float dustArea, maxHorizontalSpeed = 3, shiftDelay, firstShiftUpSpeed, secondShiftUpSpeed;
    int gearsCount = 5, curGear = 1;
    bool isUsingNos, isShifting = false;

    [SerializeField]
    AudioClip[] shiftAudioCips = null, crashAudioClips = null, nosEndAudioClips = null, turboAudioClips = null;
    AudioSource[] shiftAudioSources, crashAudioSources, nosEndAudioSources, turboAudiosSources;


    RacerPresenter racer = null;
    VelocityReader vreader = new VelocityReader();

    private void Awake()
    {
        racer = GetComponentInParent<RacerPresenter>();
    }

    public void Start()
    {
        if (GameMap.Current.Id == 0)
        {
            Destroy(gameObject);
            return;
        }

        // init audios
        engineIdleAudioSource = InitAudio(engineIdleAudioClip, true, true);
        engineSpeedUpAudioSource = InitAudio(engineSpeedUpAudioClip, true, true);
        windAudioSource = InitAudio(windAudioClip, true, false);
        sandRollingAudioSource = InitAudio(sandRollingAudioClip, true, true);
        asphaltRollingAudioSource = InitAudio(asphaltRollingAudioClip, true, true);
        asphaltSkidAudioSource = InitAudio(asphaltSkidAudioClip, true, true);
        nosStartAudioSrouce = InitAudio(nosStartAudioClip, false, false, AUDIO_RATIO);
        nosLoopAudioSource = InitAudio(nosLoopAudioClip, true, false, AUDIO_RATIO);
        hornAudioSource = InitAudio(hornAudioClip, true, false, 1, 3);

        if (turboIdleAudioClip)
            turboIdleAudioSource = InitAudio(turboIdleAudioClip, true, true);
        if (turboSpeedAudioClip)
            turboSpeedAudioSource = InitAudio(turboSpeedAudioClip, true, true);

        shiftAudioSources = new AudioSource[shiftAudioCips.Length];
        for (int i = 0; i < shiftAudioSources.Length; i++)
            shiftAudioSources[i] = InitAudio(shiftAudioCips[i], false, false, AUDIO_RATIO);

        crashAudioSources = new AudioSource[crashAudioClips.Length];
        for (int i = 0; i < crashAudioSources.Length; i++)
            crashAudioSources[i] = InitAudio(crashAudioClips[i], false, false, AUDIO_RATIO);

        nosEndAudioSources = new AudioSource[nosEndAudioClips.Length];
        for (int i = 0; i < nosEndAudioSources.Length; i++)
            nosEndAudioSources[i] = InitAudio(nosEndAudioClips[i], false, false, AUDIO_RATIO);

        turboAudiosSources = new AudioSource[turboAudioClips.Length];
        for (int i = 0; i < turboAudiosSources.Length; i++)
            turboAudiosSources[i] = InitAudio(turboAudioClips[i], false, false, AUDIO_RATIO);

        firstShiftUpSpeed = Random.Range(36, 40);
        secondShiftUpSpeed = Random.Range(50, 54);

        enabled = false;
    }

    public void EnableRacerAudio()
    {
        enabled = true;
    }

    IEnumerator ShiftUp()
    {
        isShifting = true;
        racer.IsShifting = true;
        curGear++;
        if (turboAudiosSources.Length > 0 && Random.value > .3f)
            turboAudiosSources[Random.Range(0, turboAudiosSources.Length)].Play();
        racer.BroadcastMessage("Shifting", SendMessageOptions.DontRequireReceiver);
        yield return new WaitForSeconds(.4f);
        shiftAudioSources[Random.Range(0, shiftAudioSources.Length)].Play();
        yield return new WaitForSeconds(.4f);
        isShifting = false;
        racer.IsShifting = false;
    }

    void Update()
    {
        if (racer == null || GameMap.Current == null || GameMap.Current.Id == 0) return;
        //var playTime = PlayNetwork.PlayTime;

        vreader.Update(Vector3.one.Scale(racer.transform.localPosition.x, 0, transform.position.z));
        curSpeed.x = vreader.x;
        curSpeed.z = vreader.value.magnitude;

        if (curGear == 1 && curSpeed.z > firstShiftUpSpeed)
            StartCoroutine(ShiftUp());
        else if (curGear == 2 && curSpeed.z > secondShiftUpSpeed)
            StartCoroutine(ShiftUp());

        // wind audio
        if (windAudioSource)
        {
            windAudioSource.volume = System.Math.Min(curSpeed.z / 200f, .4f) * AUDIO_RATIO;
            windAudioSource.pitch = 1 + curSpeed.z / 180;
        }

        // asphalt and sand audio
        //if (System.Math.Abs(mpTr.localPosition.x) < dustArea)
        {
            sandRollingAudioSource.volume = 0;
            asphaltRollingAudioSource.volume = System.Math.Min(curSpeed.z / 100, .8f) * AUDIO_RATIO;
            asphaltRollingAudioSource.pitch = 1 + curSpeed.z / 120;
        }
        /*else
        {
            asphaltRollingAudio.volume = 0;
            sandRollingAudio.volume = System.Math.Min(curSpeed.z / 40, .99f) * AUDIO_RATIO;
            sandRollingAudio.pitch = 1 + curSpeed.z / 60;
        }*/


        // asphalt skid audio
        asphaltSkidAudioSource.volume = (Mathf.Abs(curSpeed.x) - 2) / maxHorizontalSpeed * AUDIO_RATIO;

        // turbo audio
        if (turboIdleAudioSource && turboSpeedAudioSource)
        {
            turboIdleAudioSource.pitch = turboSpeedAudioSource.pitch = 1 + curSpeed.z / 150f;
            if (isShifting)
            {
                turboIdleAudioSource.volume = Mathf.Lerp(turboIdleAudioSource.volume, 0f, Time.deltaTime * 4f);
                turboSpeedAudioSource.volume = Mathf.Lerp(turboSpeedAudioSource.volume, .5f * AUDIO_RATIO, Time.deltaTime * 4f);
            }
            else
            {
                turboIdleAudioSource.volume = Mathf.Lerp(turboIdleAudioSource.volume, .25f * AUDIO_RATIO, Time.deltaTime * 4f);
                turboSpeedAudioSource.volume = Mathf.Lerp(turboSpeedAudioSource.volume, 0f, Time.deltaTime * 4f);
            }
        }

        // engine audio
        if (isShifting)
        {
            engineSpeedUpAudioSource.pitch = Mathf.Lerp(engineSpeedUpAudioSource.pitch, 1, Time.deltaTime * 1f);
            engineSpeedUpAudioSource.volume = Mathf.Lerp(engineSpeedUpAudioSource.volume, 0, Time.deltaTime * 5f);

            engineIdleAudioSource.pitch = Mathf.Lerp(engineIdleAudioSource.pitch, 1, Time.deltaTime * 1f);
            engineIdleAudioSource.volume = Mathf.Lerp(engineIdleAudioSource.volume, 1 * AUDIO_RATIO, Time.deltaTime * 5f);
        }
        else
        {
            float p = 1 + (curSpeed.z - (RaceModel.specs.minForwardSpeed + (RaceModel.specs.maxForwardSpeed / gearsCount) * (curGear - 1))) / 40f;
            engineSpeedUpAudioSource.pitch = Mathf.Lerp(engineSpeedUpAudioSource.pitch, p, Time.deltaTime * 10f);
            engineSpeedUpAudioSource.volume = Mathf.Lerp(engineSpeedUpAudioSource.volume, 1 * AUDIO_RATIO, Time.deltaTime * 5f);

            engineIdleAudioSource.pitch = Mathf.Lerp(engineIdleAudioSource.pitch, p, Time.deltaTime * 1f);
            engineIdleAudioSource.volume = Mathf.Lerp(engineIdleAudioSource.volume, .2f * AUDIO_RATIO, Time.deltaTime * 5f);
        }
    }

    public void PlayCrashAudio()
    {
        crashAudioSources[Random.Range(0, crashAudioSources.Length)].Play();
    }

    public void StartNitors()
    {
        isUsingNos = true;
        nosStartAudioSrouce.Play();
        DelayCall(.6f, () =>
        {
            if (isUsingNos)
                nosLoopAudioSource.Play();
        });
    }

    public void StopNitors()
    {
        if (isUsingNos)
        {
            nosLoopAudioSource.Stop();
            nosEndAudioSources[Random.Range(0, nosEndAudioSources.Length)].Play();
            isUsingNos = false;
        }
    }

    public void BoostNitors()
    {
        if (isUsingNos)
        {
            nosStartAudioSrouce.Play();
        }
    }

    public void PlayHornAudio()
    {
        hornAudioSource.Play();
    }

    public void StopHornAudio()
    {
        if (hornAudioSource.isPlaying)
            hornAudioSource.Stop();
    }


    AudioSource InitAudio(AudioClip clip, bool loop, bool playNow, float volume = 0, float minDistance = 2)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1;
        if (playNow)
            audioSource.Play();
        audioSource.minDistance = minDistance;
        return audioSource;
    }
}
