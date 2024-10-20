using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    private static Timer _instance;
    public static Timer Instance => _instance;

    [SerializeField] private float _initialTime;

    [SerializeField] private UnityEvent<string> _onTick;
    [SerializeField] private string _format;

    public UnityEvent<string> OnTick => _onTick;

    [SerializeField] private float _timeMultiplier;

    private float _currentTime;
    public float CurrentTime => _currentTime;

    private bool _isCounting;
    public void SetTimerState(bool isCounting)
    {
        _isCounting = isCounting;
    }

    public void ResetTime()
    {
        _currentTime = _initialTime;   
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _currentTime = _initialTime;
    }

    private void Update()
    {
        if (_isCounting)
        {
            _currentTime += _timeMultiplier * Time.deltaTime;

            var time = System.TimeSpan.FromSeconds(_currentTime);
            string text = new(time.ToString(_format));

            _onTick.Invoke(text);
        }
    }
}
