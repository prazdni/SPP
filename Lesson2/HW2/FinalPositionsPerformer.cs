using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class FinalPositionsPerformer : MonoBehaviour
{
    [SerializeField] private int _arraySize;
    [SerializeField] private int _maxRandPos;
    [SerializeField] private int _maxRandVel;
    private NativeArray<int> _positions;
    private NativeArray<int> _velocities;
    private NativeArray<int> _finalPositions;
    private JobHandle _positionsInitJobHandle;
    private JobHandle _velocitiesInitJobHandle;
    private JobHandle _finalPositionsJobHandle;

    private IEnumerator Start()
    {
        _positions = new NativeArray<int>(_arraySize, Allocator.TempJob);
        _velocities = new NativeArray<int>(_arraySize, Allocator.TempJob);
        _finalPositions = new NativeArray<int>(_arraySize, Allocator.TempJob);
        InitJob initPositionsJob = new InitJob { initNums = _positions, maxRand = _maxRandPos };
        _positionsInitJobHandle = initPositionsJob.Schedule();
        InitJob initVelocitiesJob = new InitJob { initNums = _velocities, maxRand = _maxRandVel };
        _velocitiesInitJobHandle = initVelocitiesJob.Schedule(_positionsInitJobHandle);
        FinalPositionsJob finalPositionsJob = new FinalPositionsJob { positions = _positions, velocities = _velocities, finalPositions = _finalPositions };
        _finalPositionsJobHandle = finalPositionsJob.Schedule(_arraySize, 5, _velocitiesInitJobHandle);
        _finalPositionsJobHandle.Complete();

         while (!_finalPositionsJobHandle.IsCompleted)
            yield return new WaitForEndOfFrame();

        for(int i = 0; i < _arraySize; i++)
            Debug.Log($"{_positions[i]} + {_velocities[i]} = {_finalPositions[i]}");

        _positions.Dispose();
        _velocities.Dispose();
        _finalPositions.Dispose();
    }
}