using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _waveCount;
    [SerializeField] private float _waveIntensityScale;
    [SerializeField] private float _waveIntensityOffset;

    [SerializeField] private float _timeConversionSpeed;

    [SerializeField] private UnityEvent<string> _onWaveChange;
    [SerializeField] private string _format;

    [SerializeField] private UnityEvent _onGameEnd;

    private int _currentWave;
    private bool _isWaveActive;

    public float WaveIntensityValue => _waveIntensityOffset + (_currentWave * _waveIntensityScale);

    private int _totalTimeSaved;
    public int TotalTimeSaved => _totalTimeSaved;

    private int _totalMiceCaptured;
    public int TotalMiceCaptured => _totalMiceCaptured;

    private void Start()
    {
        MouseObjectManager.Instance.SetupAllMouseObjects();
        _currentWave = 1;
    }

    private void Update()
    {
        bool isTimerOver = Timer.Instance.TimeMultiplier > 0 ? Timer.Instance.CurrentTime >= Timer.Instance.EndTime : Timer.Instance.CurrentTime <= Timer.Instance.EndTime;

        if (_isWaveActive && (MouseObjectManager.Instance.CurrentMouseCount == 0 || isTimerOver))
        {
            EndWave();
        }
    }

    public int GetWinner()
    {
        return FindObjectsByType<PlayerScore>(FindObjectsSortMode.None).OrderBy(item => item.CurrentScore).ElementAt(0).PlayerIndex;
    }

    [ContextMenu("Start Wave")]
    public void StartWave()
    {
        Debug.Log($"Wave {_currentWave} begun!");

        Timer.Instance.SetTimerState(true);
        MouseObjectManager.Instance.AddAllMice();

        _isWaveActive = true;
    }
    
    [ContextMenu("End Wave")]
    public void EndWave()
    {
        Debug.Log($"Wave {_currentWave} ended!");

        _isWaveActive = false;

        MouseObjectManager.Instance.RemoveAllMice();
        Timer.Instance.SetTimerState(false);
        Timer.Instance.ResetTime();

        _totalTimeSaved += (int)Timer.Instance.CurrentTime;
        _totalMiceCaptured += MouseObjectManager.Instance.CollectedMouseCount;

        StartCoroutine(ConvertTimeToScoreOverTime());
    }

    private IEnumerator ConvertTimeToScoreOverTime()
    {
        var playerScores = FindObjectsByType<PlayerScore>(FindObjectsSortMode.None);

        for (int i = 0; i < Timer.Instance.CurrentTime; ++i)
        {
            foreach (var score in playerScores)
            {
                score.ModifyScore(10);
            }

            yield return new WaitForSeconds(_timeConversionSpeed);
        }

        FinishWaveEnd();
    }

    private void FinishWaveEnd()
    {
        ++_currentWave;
        _onWaveChange.Invoke(string.Format(_format, _currentWave, _waveCount));

        if (_currentWave > _waveCount)
        {
            Debug.Log("Game ended!");

            _onGameEnd.Invoke();
        }
    }
}
