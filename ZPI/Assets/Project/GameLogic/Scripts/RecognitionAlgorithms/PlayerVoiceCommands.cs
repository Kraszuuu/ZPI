using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class PlayerVoiceCommands : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, string> actions = new Dictionary<string, string>();
    public string recognizedSpell;
    private string recognizedWord;
    public bool isOn = true;

    private void Start()
    {
        AddWords();

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();

    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        if (Input.GetMouseButton(0))
        {
            recognizedWord = speech.text;
            recognizedSpell = actions[recognizedWord];
        }
    }

    private void AddWords()
    {
        // Fireball (augue)
        actions.Add("Ohk", "Fireball");
        actions.Add("out", "Fireball");
        actions.Add("Aug", "Fireball");
        actions.Add("oak", "Fireball");
        actions.Add("outlook", "Fireball");
        actions.Add("ohh", "Fireball");

        // Meteors (meteor examen)
        actions.Add("meteor examine", "Meteors");
        actions.Add("meteora examine", "Meteors");
        actions.Add("method examine", "Meteors");
        actions.Add("meteor XMN ", "Meteors");
        actions.Add("meteora XMN ", "Meteors");
        actions.Add("method XMN", "Meteors");
        actions.Add("meteor exam ", "Meteors");
        actions.Add("meteora exam ", "Meteors");
        actions.Add("method exam", "Meteors");
        actions.Add("methodics Amen ", "Meteors");
        actions.Add("metorex Amen", "Meteors");
        actions.Add("meteor rocks Amen ", "Meteors");
        actions.Add("meteor wrecks Amen", "Meteors");
        actions.Add("meet your examine ", "Meteors");
        actions.Add("met yo examine ", "Meteors");

        // Shield (protectio)
        actions.Add("professional", "Shield");
        actions.Add("protesto", "Shield");
        actions.Add("protects you", "Shield");
        actions.Add("protection", "Shield");
        actions.Add("protect you", "Shield");
        actions.Add("protect your", "Shield");

        // Lightning (fulgur)
        actions.Add("fulgur", "Lightning");
        actions.Add("vulgar", "Lightning");
        actions.Add("full court", "Lightning");
        actions.Add("folklore", "Lightning");
        actions.Add("Faulkner", "Lightning");
        actions.Add("Fogger", "Lightning");
        actions.Add("forget", "Lightning");
        actions.Add("parkour", "Lightning");
    }
}
