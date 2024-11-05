using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject PausePanel;

    public void PauseGame()
    {
        GameState.Instance.IsGamePaused = true;
        PausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        GameState.Instance.IsGamePaused = false;
        PausePanel.SetActive(false);
    }
}
