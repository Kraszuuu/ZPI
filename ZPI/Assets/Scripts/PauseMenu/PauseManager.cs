using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject PausePanel;
    private GameFreezer gameFreezer;

    private void Start()
    {
        //gameFreezer = FindObjectOfType<GameFreezer>();
    }

    private void OnEnable()
    {
        gameFreezer = FindObjectOfType<GameFreezer>();
    }

    public void PauseGame()
    {
        PausePanel.SetActive(true);
        gameFreezer.SetIsGamePaused(true);
        Debug.Log("PAUZA");
    }

    public void ResumeGame()
    {
        PausePanel.SetActive(false);
        gameFreezer.SetIsGamePaused(false);
        Debug.Log("WZNOWIONO");
    }
}
