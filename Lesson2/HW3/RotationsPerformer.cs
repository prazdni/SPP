using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class RotationsPerformer : MonoBehaviour
{
    [SerializeField] private GameObject _rotationsGo;
    [SerializeField] private int _arraySize;
    [SerializeField] private int _speed;
    [SerializeField] private int _rand;
    private Transform[] _transforms;
    private NativeArray<Vector3> _rotations;
    private JobHandle _initJobhandle;
    private JobHandle _rotationJobHandle;
    private TransformAccessArray _transformAccessArray;

    private void Start()
    {
        _transforms = new Transform[_arraySize];
        for (int i = 0; i < _arraySize; i++)
        {
            _transforms[i] = Object.Instantiate(_rotationsGo).transform;
            _transforms[i].position = Random.insideUnitSphere * _rand;
        }
        _transformAccessArray = new TransformAccessArray(_transforms);
        _rotations = new NativeArray<Vector3>(_arraySize, Allocator.Persistent);

        InitVectorJob initJob = new InitVectorJob { array = _rotations, maxRand = _rand, sphere = Random.insideUnitSphere };
        _initJobhandle = initJob.Schedule();
        _initJobhandle.Complete();
    }

    private void Update()
    {
        RotationsJob rotationJob = new RotationsJob { rotations = _rotations, deltaTime = _speed * Time.deltaTime };
        _rotationJobHandle = rotationJob.Schedule(_transformAccessArray);
        _rotationJobHandle.Complete();
    }

    private void OnDestroy()
    {
        _rotations.Dispose();
        _transformAccessArray.Dispose();
    }
}