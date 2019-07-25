using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StableRandom
{
    private static int bseed = 0;
//    private static Random.State state;

    public static void Initialize(int baseSeed)
    {
        bseed = baseSeed;
    }

    public static void Begin(int seed)
    {
        //state = Random.state;
        Random.InitState(bseed + seed);
    }

    public static int Get(int min, int max)
    {        
        return Random.Range(min, max + 1);
    }

    public static void End()
    {
        //Random.state = state;
    }
}
