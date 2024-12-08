using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManagerMenu : MonoBehaviour
{
    public static AudioManagerMenu instance;
    public AudioClip buttonClickSound; // "ButtonClick"
    public AudioClip music;
    private AudioSource _audioSource;
    private AudioSource _musicSource;
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [SerializeField] private Slider _musicSlider;
    [Range(0f, 1f)] public float soundVolume = 0.5f;
    [SerializeField] private Slider _soundSlider;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource nie znaleziono, dodaj� nowy.");
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.volume = soundVolume;
        }
        if (_musicSource == null)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            StartPlayingMusic();
            _musicSource.volume = musicVolume * soundVolume;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayButtonSound()
    {
        if (buttonClickSound != null) _audioSource.PlayOneShot(buttonClickSound);
    }

    public void SetSoundVolume()
    {
        soundVolume = _soundSlider.value;
        _audioSource.volume = soundVolume;
        _musicSource.volume = musicVolume * soundVolume;
    }

    public void SetMusicVolume()
    {
        musicVolume = _musicSlider.value;
        _musicSource.volume = musicVolume * soundVolume;
    }

    public void StopPlayingMusic()
    {
        _musicSource.Stop();
    }

    public void StartPlayingMusic()
    {
        _musicSource.PlayOneShot(music);
    }
}
