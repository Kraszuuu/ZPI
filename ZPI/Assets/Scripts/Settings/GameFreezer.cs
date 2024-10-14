using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFreezer : MonoBehaviour
{
    private bool isCastSpelling;
    private bool isGamePaused;

    public void setIsCastSpelling(bool castSpelling)
    {
        this.isCastSpelling = castSpelling;
        UpdateTimeScale();
    }

    public void setIsGamePaused(bool isGamePaused)
    {
        this.isGamePaused = isGamePaused;
        UpdateTimeScale();
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
