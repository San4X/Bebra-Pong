using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPrediction : MonoBehaviour
{
    private float _timer;
    private int _currentTick;
    private float _minTimeBetweenTicks;

    private const float SERVER_TICK_RATE = 30f;
    void Start()
    {
        _minTimeBetweenTicks = 1f / SERVER_TICK_RATE;
    }
    
    void Update()
    {
        _timer += Time.deltaTime;

        while (_timer >= _minTimeBetweenTicks)
        {
            _timer -= _minTimeBetweenTicks;
            HandleTick();
            _currentTick++;
        }
    }

    void HandleTick()
    {
        
    }
}
