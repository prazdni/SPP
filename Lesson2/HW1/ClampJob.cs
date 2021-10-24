using Unity.Collections;
using Unity.Jobs;

public struct ClampJob : IJob
{
    public NativeArray<int> nums;
    [ReadOnly]
    public int maxNum;

    public void Execute()
    {
        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[i] > maxNum)
                nums[i] = 0;
        }
    }
}
