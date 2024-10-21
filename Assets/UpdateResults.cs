using UnityEngine;

public class UpdateResults : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;

    [SerializeField] private TMPro.TMP_Text _miceCaptured;
    [SerializeField] private TMPro.TMP_Text _timeSaved;
    [SerializeField] private TMPro.TMP_Text _totalPoints;

    [SerializeField] private int _pointsPerMouse;
    [SerializeField] private int _pointsPerSecond;

    private void OnEnable()
    {
        _miceCaptured.SetText(_gameManager.TotalMiceCaptured.ToString());
        _timeSaved.SetText(_gameManager.TotalTimeSaved.ToString());

        int points = _pointsPerMouse * _gameManager.TotalMiceCaptured + _pointsPerSecond * _gameManager.TotalTimeSaved;
        _totalPoints.SetText(points.ToString());

        Cursor.lockState = CursorLockMode.None;
    }
}
