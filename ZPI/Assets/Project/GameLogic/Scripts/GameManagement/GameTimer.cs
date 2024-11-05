using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private float totalGameTime = 0f;

    private void Start()
    {
    }

    private void Update()
    {
        if (!GameState.Instance.IsGamePaused)
        {
            totalGameTime += Time.deltaTime;
        }
    }

    public float GetTotalGameTime()
    {
        return totalGameTime;
    }
}
