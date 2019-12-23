using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    private int lastline = 5;

    private int currPosition { get { return Mathf.RoundToInt(RaceModel.stats.playerPosition); } }

    private void Awake()
    {
        if (RaceModel.obstacle.id == 0)
            Destroy(this);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => RaceModel.specs.maxForwardSpeed > 0);

        var maxDistance = Mathf.RoundToInt(RaceModel.obstacle.baseDistance + RaceModel.obstacle.distanceRatio * RaceModel.specs.maxForwardSpeed);
        var lastseed = currPosition / maxDistance;

        var waitTime = new WaitForSeconds(0.1f);
        while (true)
        {
            yield return waitTime;
            var randomseed = currPosition / maxDistance;
            if (randomseed == lastseed) continue;

            StableRandom.Begin(lastseed = randomseed);
            float lineOffset = GlobalConfig.Race.obstacle.positionVariance * StableRandom.Get(-100, 100) / 100.0f;
            float roadWidth = RoadPresenter.RoadWidth * GlobalConfig.Race.obstacle.roadWidthFactor;
            float distanceVariance = GlobalConfig.Race.obstacle.distanceVariance * StableRandom.Get(-100, 100) / 100.0f;
            float line = GetNewLine() * roadWidth;
            StableRandom.End();

            GlobalFactory.Obstacles.Create(RaceModel.obstacle.id, line + lineOffset, distanceVariance, transform);
        }
    }

    private int GetNewLine()
    {
        var res = lastline;
        while (res == lastline)
            res = StableRandom.Get(-1, 1);
        return lastline = res;
    }
}
