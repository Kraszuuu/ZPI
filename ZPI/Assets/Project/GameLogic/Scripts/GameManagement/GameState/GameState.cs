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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}