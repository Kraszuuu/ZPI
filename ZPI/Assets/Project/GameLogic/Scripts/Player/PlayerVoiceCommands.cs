using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class PlayerVoiceCommands : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    private SpellCasting spellCasting;

    private void Start()
    {
        spellCasting = FindObjectOfType<SpellCasting>();
        AddWords();

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();

    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        if (Input.GetMouseButton(0))
        {
            while (Input.GetMouseButton(0))
            {

            }

            Debug.Log("You said (GESTURE RECOGNIZED): " + speech.text);
            actions[speech.text].Invoke();
        }
        else
        {
            Debug.Log("You said (no gesture recognition): " + speech.text);
            actions[speech.text].Invoke();
        }
    }

    private void AddWords()
    {
        actions.Add("Fireball", spellCasting.CastFireSpellInDirection);
    }
}
