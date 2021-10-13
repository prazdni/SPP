using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    private const int _maxHealth = 100;
    private const int _healthAddition = 5;
    private const float _counter = 0.5f;
    private const float _maxHealingTime = 3.0f;

    [SerializeField] private int _health;

    private Coroutine _co;
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(_counter);

    public void ReceiveHealing()
    {
        if (_co == null)
            _co = StartCoroutine(ReceiveHealingInternal());
    }

    private IEnumerator ReceiveHealingInternal()
    {
        float timer = _maxHealingTime;
        while ( _health < _maxHealth || timer > 0 )
        {
            _health += _healthAddition;
            timer -= _counter;

            yield return waitForSecondsRealtime;
        }

        _co = null;
    }
}