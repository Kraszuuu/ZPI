using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class GameFreezer : MonoBehaviour
{
    private InputManager inputManager;
    private float targetTimeScale = 1f;
    private float timeScaleSpeed = 2f; // Prêdkoœæ liniowej interpolacji
    private float minTimeScale = 0.1f; // Minimalna wartoœæ spowolnienia czasu

    private void Start()
    {
        inputManager = GameObject.Find("Player").GetComponent<InputManager>();
        StartCoroutine(UpdateTimeScaleCoroutine());
    }

    private void OnEnable()
    {
        inputManager = GameObject.Find("Player").GetComponent<InputManager>();
    }

    public IEnumerator UpdateTimeScaleCoroutine()
    {
        while (true)
        {
            // Okreœlenie docelowej wartoœci Time.timeScale w zale¿noœci od stanu gry
            if (GameState.Instance.IsSpellCasting)
            {
                targetTimeScale = minTimeScale;
                inputManager.enabled = false;
            }
            else if (GameState.Instance.IsGamePaused || GameState.Instance.IsGameOver)
            {
                targetTimeScale = 0f;
            }
            else
            {
                targetTimeScale = 1f;
                inputManager.enabled = true;
            }

            // Liniowe przejœcie do docelowej wartoœci Time.timeScale
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, targetTimeScale, timeScaleSpeed * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            // Odczekaj jedn¹ klatkê przed kolejnym wywo³aniem
            yield return null;
        }
    }
}
