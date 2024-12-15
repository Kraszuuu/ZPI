using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using UnityEngine.UI;

public class PlayerVoiceCommands : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, string> actions = new Dictionary<string, string>();
    public string recognizedSpell;
    public string recognizedWord;
    public TMP_Text recognizedWordText;

    private void Start()
    {
        Debug.Log($"Speech recognition initialized to: {GameState.Instance.IsSpeechRecognitionEnabled}");
        recognizedWordText.text = "";

        AddWords();

        try
        {
            // Try initializing the KeywordRecognizer
            keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
            keywordRecognizer.Start();
            Debug.Log("Speech recognition started successfully.");
        }
        catch (Exception ex)
        {
            // Log a debug message if speech recognition is not supported
            Debug.LogWarning($"Speech recognition is not supported on this machine: {ex.Message}");
        }
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        if (Input.GetMouseButton(1))
        {
            recognizedWord = speech.text;
            recognizedWordText.text = recognizedWord;
            recognizedSpell = actions[recognizedWord];
            Debug.Log("You said " + recognizedWord);
        }
        else
        {
            Debug.Log("RMB not pressed " + speech.text);
        }
    }

    private void AddWords()
    {
        // Fireball (augue)
        //actions.Add("Ohk", "Fireball");
        //actions.Add("out", "Fireball");
        //actions.Add("Aug", "Fireball");
        //actions.Add("oak", "Fireball");
        //actions.Add("outlook", "Fireball");
        //actions.Add("ohh", "Fireball");
        //actions.Add("I�ll get", "Fireball");
        //actions.Add("out of", "Fireball");
        //actions.Add("our debt", "Fireball");
        //actions.Add("Of it", "Fireball");
        //actions.Add("all day", "Fireball");
        //actions.Add("out a", "Fireball");
        actions.Add("Fireball", "Fireball");

        // Meteors (meteor examen)
        //actions.Add("meteor examine", "Meteors");
        //actions.Add("meteora examine", "Meteors");
        //actions.Add("method examine", "Meteors");
        //actions.Add("meteor XMN ", "Meteors");
        //actions.Add("meteora XMN ", "Meteors");
        //actions.Add("method XMN", "Meteors");
        //actions.Add("meteor exam ", "Meteors");
        //actions.Add("meteora exam ", "Meteors");
        //actions.Add("method exam", "Meteors");
        //actions.Add("methodics Amen ", "Meteors");
        //actions.Add("metorex Amen", "Meteors");
        //actions.Add("meteor rocks Amen ", "Meteors");
        //actions.Add("meteor wrecks Amen", "Meteors");
        //actions.Add("meet your examine ", "Meteors");
        //actions.Add("met yo examine ", "Meteors");
        actions.Add("Meteors", "Meteors");

        // Shield (protectio)
        //actions.Add("professional", "Shield");
        //actions.Add("protesto", "Shield");
        //actions.Add("protects you", "Shield");
        //actions.Add("protection", "Shield");
        //actions.Add("protect you", "Shield");
        //actions.Add("protect your", "Shield");
        actions.Add("Shield", "Shield");

        // Lightning (fulgur)
        //actions.Add("fulgur", "Lightning");
        //actions.Add("vulgar", "Lightning");
        //actions.Add("full court", "Lightning");
        //actions.Add("folklore", "Lightning");
        //actions.Add("Faulkner", "Lightning");
        //actions.Add("Fogger", "Lightning");
        //actions.Add("forget", "Lightning");
        //actions.Add("parkour", "Lightning");
        actions.Add("Lightning", "Lightning");
    }
}
