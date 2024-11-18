using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject PausePanel;
    public bool isEnabled = true;

    private void Start()
    {

    }

    public void PauseGame()
    {
        if (isEnabled && !GameState.Instance.IsGameOver)
        {
            GameState.Instance.IsGamePaused = true;
            Time.timeScale = 0f;
            PausePanel.SetActive(true);
            GameState.Instance.IsGamePaused = true;
            Cursor.lockState = CursorLockMode.Confined;
            AudioManager.instance.PlayPauseSound();
        }
    }

    public void ResumeGame()
    {
        if (isEnabled)
        {
            GameState.Instance.IsGamePaused = false;
            Time.timeScale = 1f;
            PausePanel.SetActive(false);
            GameState.Instance.IsGamePaused= false;
            AudioManager.instance.PlayUnpauseSound();
        }
    }
}
