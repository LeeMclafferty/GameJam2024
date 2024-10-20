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

    public void ModifyScore(int amount)
    {
        Debug.Log($"Player {_playerIndex + 1} score increased by {amount}");

        _currentScore += amount;
        _onChange.Invoke(string.Format(_format, _currentScore));

        if (_currentScore > PlayerPrefs.GetInt("HiScore"))
        {
            PlayerPrefs.SetInt("HiScore", _currentScore);
        }
    }
}
