using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayingBehindDistance : MonoBehaviour
{
    [SerializeField] private GameObject holder = null;
    [SerializeField] private Text label = null;
    [SerializeField] private float minDistance = 4;
    [SerializeField] private float positionSpeed = 20;
    [SerializeField] private float positionRange = 450;

    private RectTransform rectTransform = null;
    private BlenderValue position = new BlenderValue();

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        holder.SetActive(false);
        position.speed = positionSpeed;
        position.Setup(rectTransform.anchoredPosition.x);
    }

    private IEnumerator Start()
    {
        var waitTime = new WaitForSeconds(0.1f);

        while (RaceModel.stats.playTime < RaceModel.specs.maxPlayTime)
        {
            UpdateVisual();
            yield return waitTime;
        }
    }

    private void Update()
    {
        if (position.Update(Time.deltaTime))
            rectTransform.SetAnchordPositionX(position.current);
    }

    private void UpdateVisual()
    {
        // verify that player is not latest
        var local = PlayerPresenter.local;
        var playerIndex = PlayerPresenter.all.IndexOf(local);
        if (playerIndex == PlayerPresenter.all.Count - 1)
        {
            RaceModel.stats.playerBehindDistance = 0;
            holder.SetActive(false);
            return;
        }

        // find behind racer
        var opponent = PlayerPresenter.all[playerIndex + 1];

        // compute distance
        RaceModel.stats.playerBehindDistance = local.player.CurrPosition - opponent.player.CurrPosition;
        if (RaceModel.stats.playerMaxBehindDistance < RaceModel.stats.playerBehindDistance)
            RaceModel.stats.playerMaxBehindDistance = RaceModel.stats.playerBehindDistance;
        if (RaceModel.stats.playerBehindDistance < minDistance)
        {
            holder.SetActive(false);
            return;
        }

        // display distance
        label.text = RaceModel.stats.playerBehindDistance.ToString("0.0") + "m";

        // update position
        var pos = opponent.racer.transform.position.x - Camera.main.transform.position.x;
        position.destination = Mathf.Clamp(pos * positionRange, -500, 500);

        holder.SetActive(true);
    }
}
