using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WhatTaskFaster : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Tasks _tasks;
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

    private async void StartExecuting()
    {
        CancellationToken cancelToken = _cancellationTokenSource.Token;
        Task<bool> task1 = Task.Run(() => _tasks.Task1Async(cancelToken, 1));
        Task<bool> task2 = Task.Run(() => _tasks.Task2Async(cancelToken, 60));
        Task<bool> result = await Task.WhenAny(task1, task2);
        Debug.Log(result.Result);
    }

    private void CancelToken()
    {
        _cancellationTokenSource.Cancel();
    }
}