using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Tasks : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _closeButton;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private void Start()
    {
        _startButton.onClick.AddListener(StartExecuting);
        _closeButton.onClick.AddListener(CancelToken);
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Dispose();
        _startButton.onClick.RemoveListener(StartExecuting);
        _closeButton.onClick.RemoveListener(CancelToken);
    }

    private void StartExecuting()
    {
        CancellationToken cancelToken = _cancellationTokenSource.Token;
        RunTask1Async(cancelToken, 1);
        RunTask2Async(cancelToken, 60);
    }

    private void CancelToken()
    {
        _cancellationTokenSource.Cancel();
    }

    private async void RunTask1Async(CancellationToken cancelToken, int seconds)
    {
        bool result = await Task.Run(() => Task1Async(cancelToken, seconds));
        Debug.Log(result);
    }

    public async Task<bool> Task1Async(CancellationToken cancelToken, int seconds)
    {
        if (cancelToken.IsCancellationRequested)
                return false;

        await Task.Delay(seconds * 1000);

        return true;
    }

    private async void RunTask2Async(CancellationToken cancelToken, int framesCount)
    {
        bool result = await Task.Run(() => Task2Async(cancelToken, framesCount));
        Debug.Log(result);
    }

    public async Task<bool> Task2Async(CancellationToken cancelToken, int framesCount)
    {
        for (int i = 0; i < framesCount; i++)
        {
            if (cancelToken.IsCancellationRequested)
                return false;

            await Task.Yield();
        }

        return false;
    }
}