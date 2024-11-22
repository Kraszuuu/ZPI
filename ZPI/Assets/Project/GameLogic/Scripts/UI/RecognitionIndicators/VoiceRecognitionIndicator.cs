using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceRecognitionIndicator : MonoBehaviour
{
    public Canvas spellCastingCanvas;

    void Update()
    {
        if (GameState.Instance != null)
        {
            spellCastingCanvas.enabled = GameState.Instance.IsSpellCasting;
        }
    }
}
