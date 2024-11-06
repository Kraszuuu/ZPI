using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject PausePanel;

    public void PauseGame()
    {
        GameState.Instance.IsGamePaused = true;
        Time.timeScale = 0f;
        PausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        GameState.Instance.IsGamePaused = false;
        Time.timeScale = 1f;
        PausePanel.SetActive(false);
    }
}
