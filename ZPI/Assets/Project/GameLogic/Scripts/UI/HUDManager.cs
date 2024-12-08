using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text WaveText; // TMP_Text dla numeru fali
    public TMP_Text TimerText; // TMP_Text dla czasu gry

    private WavesManager _wavesManager;
    private GameTimer _gameTimer;

    private void Awake()
    {
        _wavesManager = FindObjectOfType<WavesManager>();
        _gameTimer = FindObjectOfType<GameTimer>();
    }

    private void Update()
    {
        UpdateWaveText();
        UpdateTimerText();
    }

    private void UpdateWaveText()
    {
        if (_wavesManager != null && WaveText != null)
        {
            WaveText.text = $"{_wavesManager.GetCurrentWave()}";
        }
    }

    private void UpdateTimerText()
    {
        if (_gameTimer != null && TimerText != null)
        {
            float totalTime = _gameTimer.GetTotalGameTime();
            int minutes = Mathf.FloorToInt(totalTime / 60);
            int seconds = Mathf.FloorToInt(totalTime % 60);
            TimerText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}
