using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    [SerializeField] private AudioClip ambientClip; // Klip dŸwiêku ambientowego
    private AudioSource audioSource;

    private void Awake()
    {
        // Dodaj AudioSource do obiektu, jeœli go brakuje
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource missing");
        }

        // Konfiguracja AudioSource
        audioSource.clip = ambientClip; // Przypisz dŸwiêk
        audioSource.loop = true;       // W³¹cz pêtlê
        audioSource.playOnAwake = true; // Odtwarzaj automatycznie przy uruchomieniu
    }

    private void Start()
    {
        // Odtwórz dŸwiêk ambientowy
        if (ambientClip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Brak przypisanego klipu dŸwiêkowego w AmbientSoundManager.");
        }
    }
}
