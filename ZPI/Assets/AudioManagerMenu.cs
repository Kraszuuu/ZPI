using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerMenu : MonoBehaviour
{
    public static AudioManagerMenu instance;
    public AudioClip buttonClickSound; // "ButtonClick"
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource nie znaleziono, dodajê nowy.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayButtonSound()
    {
        if (buttonClickSound != null) audioSource.PlayOneShot(buttonClickSound); 
    }
}
