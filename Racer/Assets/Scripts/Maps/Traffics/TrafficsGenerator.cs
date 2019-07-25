using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficsGenerator : MonoBehaviour
{
    private int lastline = 5;

    private int currPosition { get { return Mathf.RoundToInt(PlayerPresenter.local.ForwardValue); } }

    private void OnEnable()
    {
        GlobalFactory.TrafficCars.CreatePool();
    }

    private void OnDisable()
    {
        GlobalFactory.TrafficCars.ReleasePool();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5);

        var maxDistance = Mathf.RoundToInt(GlobalConfig.Race.traffics.baseDistance + GlobalConfig.Race.traffics.speedFactor * PlayModel.maxForwardSpeed);
        var lastseed = currPosition / maxDistance;
        var waitTime = new WaitForSeconds(0.1f);
        while (true)
        {
            yield return waitTime;
            var randomseed = currPosition / maxDistance;
            if (randomseed == lastseed) continue;

            StableRandom.Begin(lastseed = randomseed);
            int car1 = StableRandom.Get(0, 1000);
            int car2 = StableRandom.Get(0, 1000);
            bool doubleCar = StableRandom.Get(0, 100) < GlobalConfig.Race.traffics.doubleCarChance;
            float lineOffset = GlobalConfig.Race.traffics.positionVariance * StableRandom.Get(-100, 100) / 100.0f;
            float roadWidth = RoadPresenter.RoadWidth * GlobalConfig.Race.traffics.roadWidthFactor;
            float distanceVariance = GlobalConfig.Race.traffics.distanceVariance * StableRandom.Get(-100, 100) / 100.0f;
            float line = GetNewLine() * roadWidth;
            StableRandom.End();

            if (doubleCar)
            {
                line = roadWidth + lineOffset;
                GlobalFactory.TrafficCars.Create(car1, -line, distanceVariance, transform);
                GlobalFactory.TrafficCars.Create(car2, line, distanceVariance, transform);

            }
            else GlobalFactory.TrafficCars.Create(car1, line + lineOffset, distanceVariance, transform);
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
