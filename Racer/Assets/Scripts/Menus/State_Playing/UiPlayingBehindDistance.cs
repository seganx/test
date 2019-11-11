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
        var local = PlayerPresenter.local;
        var playerIndex = PlayerPresenter.all.IndexOf(local);
        var nextOpp = PlayerPresenter.all[Mathf.Clamp(playerIndex - 1, 0, PlayerPresenter.all.LastIndex())];
        var prevOpp = PlayerPresenter.all[Mathf.Clamp(playerIndex + 1, 0, PlayerPresenter.all.LastIndex())];

        // compute distance
        RaceModel.stats.playerForwardDistance = nextOpp.player.CurrPosition - local.player.CurrPosition;
        RaceModel.stats.playerBehindDistance = local.player.CurrPosition - prevOpp.player.CurrPosition;

        // display distance
        if (RaceModel.stats.playerBehindDistance > minDistance)
        {
            label.text = RaceModel.stats.playerBehindDistance.ToString("0.0") + "m";
            var pos = prevOpp.racer.transform.position.x - Camera.main.transform.position.x;
            position.destination = Mathf.Clamp(pos * positionRange, -500, 500);
            holder.SetActive(true);
        }
        else holder.SetActive(false);
    }
}
