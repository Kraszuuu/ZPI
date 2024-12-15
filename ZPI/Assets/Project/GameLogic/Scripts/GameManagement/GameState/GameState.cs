using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public bool IsSpellCasting { get; set; }
    public bool IsGamePaused { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsTimeSlowed { get; set; }
    public bool IsUpgrading { get; set; }
    public bool IsWandEquipped { get; set; }
    public bool IsSpeechRecognitionEnabled { get; set; }
    public bool IsPrimaryAttackEnabled { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Metoda statyczna do automatycznego utworzenia instancji, jeśli jej brak
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstanceExists()
    {
        if (Instance == null)
        {
            GameObject gameStateObject = new GameObject("GameState");
            gameStateObject.AddComponent<GameState>();
        }
    }
}
