using UnityEngine;
using UnityEngine.Events;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private int _playerIndex;
    public int PlayerIndex => _playerIndex;

    [SerializeField] private UnityEvent<string> _onChange;
    [SerializeField] private string _format;

    private int _currentScore;
    public int CurrentScore => _currentScore;

    public void GainPoints(int amount)
    {
        _currentScore += amount;
        _onChange.Invoke(string.Format(_format, _currentScore));
    }
}
