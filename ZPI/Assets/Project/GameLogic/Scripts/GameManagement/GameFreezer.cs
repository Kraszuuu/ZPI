using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class GameFreezer : MonoBehaviour
{
    private InputManager inputManager;

    private void Start()
    {
        inputManager = GameObject.Find("Player").GetComponent<InputManager>();
    }

    public void UpdateTimeScale()
    {
        if (GameState.Instance.IsSpellCasting || GameState.Instance.IsGamePaused)
        {
            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            inputManager.enabled = false;
        }

        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            inputManager.enabled = true;
        }
    }
}
