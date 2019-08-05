using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicPlayer : Base
{
    #region fields
    [SerializeField] private int sceneId;
    [SerializeField] private float fadeDuration;
    [SerializeField] private bool isMenuMusic = false;
    #endregion

    #region methods
    void Start()
    {
        float delay = 0;

        if (!isMenuMusic)
            delay = 10;

        DelayCall(delay, () =>
        {
            AudioManager.Instance.PlaySceneMusic(sceneId, fadeDuration);
        });
    }
    #endregion
}