using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> audioClips;
    private Dictionary<string, AudioClip> clipDictionary;
    private AudioSource audioSource;

    private const float defaultVolume = 0.5f;

    // Singleton dla AudioManager
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        clipDictionary = new Dictionary<string, AudioClip>();

        foreach (var clip in audioClips)
        {
            clipDictionary[clip.name] = clip;
        }

        SetVolume(GetSavedVolume());
    }

    public void WalkSound() => PlaySound("03_Step_grass_03");
    public void JumpSound() => PlaySound("30_Jump_03");
    public void MeteorRainSound() => PlayRandomWandSound();
    public void LightiningSound()
    {
        PlayRandomWandSound();
        PlaySound("18_Thunder_02");
    }
    public void ShieldSound()
    {
        PlayRandomWandSound();
        PlaySound("21_Debuff_01");
    }
    public void PlayerDashSound() => PlaySound("35_Miss_Evade_02");
    public void EndOfWaveSound() => PlaySound("DM-CGS-45");
    public void PauseSound() => PlaySound("092_Pouse_04");
    public void UnpauseSound() => PlaySound("098_Unpause_04");
    public void TakeDamageSound() => PlaySound("39_Block_03");
    public void PlayerDieSound() => PlaySound("DM-CGS-17");
    public void PlayerHealSound() => PlaySound("02_Heal_02");

    private void PlayRandomWandSound()
    {
        if (Random.Range(0, 2) == 0) PlaySound("DM-CGS-46");
        else PlaySound("DM-CGS-47");
    }

    private void PlaySound(string clipName)
    {
        if (clipDictionary.ContainsKey(clipName))
        {
            audioSource.PlayOneShot(clipDictionary[clipName], audioSource.volume);
        }
        else
        {
            Debug.LogWarning("Clip not found: " + clipName);
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
        SaveVolume(volume);
    }

    private void SaveVolume(float volume)
    {
        PlayerPrefs.SetFloat("AudioVolume", volume);
        PlayerPrefs.Save();
    }

    public float GetSavedVolume()
    {
        return PlayerPrefs.HasKey("AudioVolume") ? PlayerPrefs.GetFloat("AudioVolume") : defaultVolume;
    }

    public void MusicVolume(float volume)
    {
        Debug.Log("Bagno");
    }
}
