using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

public struct RotationsJob : IJobParallelForTransform
{
    public NativeArray<Vector3> rotations;
    [ReadOnly]
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        rotations[index] = rotations[index] + Vector3.one * deltaTime;
        transform.rotation = Quaternion.Euler(rotations[index]);
    }
}
