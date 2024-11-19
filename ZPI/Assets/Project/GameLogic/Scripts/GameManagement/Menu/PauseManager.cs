using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject HelpPanel;
    //public bool isEnabled = true;

    private void Start()
    {

    }

    public void PauseGame()
    {
        if (!GameState.Instance.IsGameOver)
        {
            GameState.Instance.IsGamePaused = true;
            Time.timeScale = 0f;
            Debug.Log(Time.timeScale);
            PausePanel.SetActive(true);
            GameState.Instance.IsGamePaused = true;
            Cursor.lockState = CursorLockMode.Confined;
            AudioManager.instance.PlayPauseSound();
        }
    }

    public void ResumeGame()
    {
        GameState.Instance.IsGamePaused = false;
        Time.timeScale = 1f;
        PausePanel.SetActive(false);
        GameState.Instance.IsGamePaused= false;
        AudioManager.instance.PlayUnpauseSound();
    }

    public void DisplayHelpMenu()
    {
        PausePanel.SetActive(false);
        HelpPanel.SetActive(true);
        AudioManager.instance.PlayButtonClickSound();
    }

    public void HideHelpMenu()
    {
        PausePanel.SetActive(true);
        HelpPanel.SetActive(false);
        AudioManager.instance.PlayButtonClickSound();
    }
}
