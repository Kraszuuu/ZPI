using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFreezer : MonoBehaviour
{
    private bool isCastSpelling;
    private bool isGamePaused;

    public void SetIsCastSpelling(bool castSpelling)
    {
        this.isCastSpelling = castSpelling;
        UpdateTimeScale();
    }

    public void SetIsGamePaused(bool isGamePaused)
    {
        this.isGamePaused = isGamePaused;
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
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
