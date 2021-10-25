using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ClampPerformer : MonoBehaviour
{
    [SerializeField] private int _arraySize;
    [SerializeField] private int _maxRand;
    [SerializeField] private int _maxNum;
    private NativeArray<int> _nums;
    private JobHandle _initJobhandle;
    private JobHandle _clampJobHandle;

    private IEnumerator Start()
    {
        _nums = new NativeArray<int>(_arraySize, Allocator.TempJob);
        InitJob initJob = new InitJob { initNums = _nums, maxRand = _maxRand };
        _initJobhandle = initJob.Schedule();
        
        ClampJob clampJob = new ClampJob { nums = _nums, maxNum = _maxNum };
        _clampJobHandle = clampJob.Schedule(_initJobhandle);
        _clampJobHandle.Complete();

         while (!_clampJobHandle.IsCompleted)
            yield return new WaitForEndOfFrame();

        for(int i = 0; i < _arraySize; i++)
            Debug.Log(_nums[i]);
        
        _nums.Dispose();
    }
}
