using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _waveCount;

    [SerializeField] private UnityEvent<string> _onWaveChange;
    [SerializeField] private string _format;

    private int _currentWave;
    private bool _isWaveActive;
    
    private void Start()
    {
        MouseObjectManager.Instance.SetupAllMouseObjects();
    }

    private void Update()
    {
        if (_isWaveActive && (MouseObjectManager.Instance.CurrentMouseCount == 0 || Timer.Instance.CurrentTime <= 0))
        {
            EndWave();
        }
    }

    public int GetWinner()
    {
        return FindObjectsByType<PlayerScore>(FindObjectsSortMode.None).OrderBy(item => item.CurrentScore).ElementAt(0).PlayerIndex;
    }

    public void StartWave()
    {
        Timer.Instance.SetTimerState(true);
        MouseObjectManager.Instance.AddAllMice();

        _isWaveActive = true;
    }

    public void EndWave()
    {
        _isWaveActive = false;

        MouseObjectManager.Instance.RemoveAllMice();
        Timer.Instance.SetTimerState(false);
        Timer.Instance.ResetTime();

        _onWaveChange.Invoke(string.Format(_format, _currentWave, _waveCount));
    }
}
