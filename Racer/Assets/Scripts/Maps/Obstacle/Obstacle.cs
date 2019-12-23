using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IResource
{
    public Sprite hudImage = null;

    public int Id { get; set; }

    public Obstacle Setup(int color, float line, float distanceVariance)
    {
        var forwardPosition = RaceModel.stats.playerPosition + GlobalConfig.Race.traffics.startDistance + distanceVariance;
        transform.forward = RoadPresenter.GetForwardByDistance(forwardPosition);
        transform.position = RoadPresenter.GetPositionByDistance(forwardPosition) + transform.right * line;
        return this;
    }

    private void Update()
    {
        var distance = RaceModel.stats.playerPosition - transform.position.z;
        if (distance > 50) Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerPresenter>();
        if (player != PlayerPresenter.local) return;
        RaceModel.stats.totalObstacles++;
        Destroy(gameObject);
    }
}
