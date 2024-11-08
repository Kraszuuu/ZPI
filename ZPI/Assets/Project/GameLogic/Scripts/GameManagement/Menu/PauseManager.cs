using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject PausePanel;
    private GameFreezer gameFreezer;

    private void Start()
    {

    }

    private void OnEnable()
    {
        gameFreezer = FindObjectOfType<GameFreezer>();
    }

    public void PauseGame()
    {
        GameState.Instance.IsGamePaused = true;
        Time.timeScale = 0f;
        PausePanel.SetActive(true);
        gameFreezer.SetIsGamePaused(true);
        Cursor.lockState = CursorLockMode.Confined;
        Debug.Log("PAUZA");
    }

    public void ResumeGame()
    {
        GameState.Instance.IsGamePaused = false;
        Time.timeScale = 1f;
        PausePanel.SetActive(false);
        gameFreezer.SetIsGamePaused(false);
        Debug.Log("WZNOWIONO");
    }
}
