using Unity.Collections;
using System;
using Unity.Jobs;
using UnityEngine;

public struct InitVectorJob : IJob
{
    public NativeArray<Vector3> array;
    [ReadOnly]
    public Vector3 sphere;
    [ReadOnly]
    public int maxRand;

    public void Execute()
    {
        System.Random rand = new System.Random();
        for(int i = 0; i < array.Length; i++)
            array[i] = sphere * rand.Next(0, maxRand);
    }
}
