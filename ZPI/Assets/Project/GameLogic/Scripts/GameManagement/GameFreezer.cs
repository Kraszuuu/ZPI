using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class GameFreezer : MonoBehaviour
{
    private bool isCastSpelling;
    private bool isGamePaused;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = GameObject.Find("Player").GetComponent<InputManager>();
    }

    public void SetIsCastSpelling(bool castSpelling)
    {
        this.isCastSpelling = castSpelling;
        UpdateTimeScale();
    }

    public void SetIsGamePaused(bool gamePaused)
    {
        this.isGamePaused = gamePaused;
        UpdateTimeScale();
    }

    public bool IsCastSpelling()
    {
        return isCastSpelling;
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    private void UpdateTimeScale()
    {
        if (isCastSpelling || isGamePaused)
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
