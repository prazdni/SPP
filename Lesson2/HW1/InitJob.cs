using Unity.Collections;
using Unity.Jobs;
using System;

public struct InitJob : IJob
{
    [WriteOnly]
    public NativeArray<int> initNums;
    [ReadOnly]
    public int maxRand;

    public void Execute()
    {
        Random rand = new Random();
        for (int i = 0; i < initNums.Length; i++)
            initNums[i] = rand.Next(0, maxRand);
    }
}
