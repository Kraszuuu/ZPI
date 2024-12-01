using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    [SerializeField] private AudioClip ambientClip; // Klip d�wi�ku ambientowego
    private AudioSource audioSource;

    private void Awake()
    {
        // Dodaj AudioSource do obiektu, je�li go brakuje
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource missing");
        }

        // Konfiguracja AudioSource
        audioSource.clip = ambientClip; // Przypisz d�wi�k
        audioSource.loop = true;       // W��cz p�tl�
        audioSource.playOnAwake = true; // Odtwarzaj automatycznie przy uruchomieniu
    }

    private void Start()
    {
        // Odtw�rz d�wi�k ambientowy
        if (ambientClip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Brak przypisanego klipu d�wi�kowego w AmbientSoundManager.");
        }
    }
}
