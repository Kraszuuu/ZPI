using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private float totalGameTime = 0f;

    private GameFreezer gameFreezer;

    private void Start()
    {
        gameFreezer = FindObjectOfType<GameFreezer>();
        InvokeRepeating("LogCurrentGameTime", 5f, 5f);
    }

    private void Update()
    {
        if (!GameState.Instance.IsGamePaused && !GameState.Instance.IsGameOver)
        {
            totalGameTime += Time.deltaTime;
        }
    }

    public float GetTotalGameTime()
    {
        return totalGameTime;
    }

    private void LogCurrentGameTime()
    {
        //Debug.Log($"Czas gry: {totalGameTime} sekund");
    }
}
