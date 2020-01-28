using UnityEngine;

public class BoolBuffer
{
    [SerializeField] private bool _value;

    private bool _isBuffering;
    private float _maxTimer = .2f;
    private float _currentTimer;

    public BoolBuffer(float timer)
    {
        _maxTimer = timer;
        _currentTimer = timer;
        _isBuffering = false;
        _value = false;
    }

    public void Update(float deltaTime)
    {
        if (!_isBuffering)
            return;

        _currentTimer -= deltaTime;

        if (_currentTimer <= 0f)
            Reset();
    }

    public void Reset()
    {
        _isBuffering = false;
        _currentTimer = -1;
        _value = false;
    }

    public void Trigger()
    {
        _isBuffering = true;
        _currentTimer = _maxTimer;
        _value = true;
    }

    public bool Value => _value;
}
