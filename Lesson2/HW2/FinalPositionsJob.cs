using Unity.Collections;
using Unity.Jobs;

public struct FinalPositionsJob : IJobParallelFor
{
    [WriteOnly]
    public NativeArray<int> finalPositions;
    [ReadOnly]
    public NativeArray<int> positions;
    [ReadOnly]
    public NativeArray<int> velocities;

    public void Execute(int index)
    {
        finalPositions[index] = positions[index] + velocities[index];
    }
}
