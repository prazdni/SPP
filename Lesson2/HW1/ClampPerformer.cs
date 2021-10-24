using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ClampPerformer : MonoBehaviour
{
    [SerializeField] private int arraySize;
    [SerializeField] private int maxRand;
    [SerializeField] private int maxNum;
    private NativeArray<int> nums;
    private JobHandle _initJobhandle;
    private JobHandle _clampJobHandle;

    private IEnumerator Start()
    {
        nums = new NativeArray<int>(arraySize, Allocator.TempJob);
        InitJob initJob = new InitJob { initNums = nums, maxRand = maxRand };
        _initJobhandle = initJob.Schedule();
        
        ClampJob clampJob = new ClampJob { nums = nums, maxNum = maxNum };
        _clampJobHandle = clampJob.Schedule(_initJobhandle);
        _clampJobHandle.Complete();

         while (!_clampJobHandle.IsCompleted)
            yield return new WaitForEndOfFrame();

        for(int i = 0; i < arraySize; i++)
            Debug.Log(nums[i]);
        
        nums.Dispose();
    }
}
