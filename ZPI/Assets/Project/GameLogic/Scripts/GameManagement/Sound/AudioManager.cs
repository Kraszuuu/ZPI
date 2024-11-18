using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource playerSource;

    [Header("Audio Clips")]
    public AudioClip buttonClickSound; // "ButtonClick"
    public AudioClip jumpSound; //"51_Flee_02"  "45_landing_01" "30_Jump_03"
    public AudioClip pauseSound; //"001_Hoover_01" "092_Pause_04"
    public AudioClip unpasueSound; // "098_Unpause_04"
    public AudioClip lightningSound; // "18_Thunder_02"
    public AudioClip stupefySound; // "77_flesh_02" "22_Slash_04" "56_Attack_03"
    public AudioClip meteorRainSound; // "dropping-rocks-5996"
    public AudioClip shieldSound; // "21_Debuff_01" "39_Absorb_04"
    public AudioClip walkSound; // "03_Step_grass_03"
    public AudioClip playerDamageSound; // "39_Block_03" "08_Bite_04"
    public AudioClip playerDeathSound; // "DM-CGS-178" "male-death-sound"
    public AudioClip wandSound1; // "DM-CGS-46"
    public AudioClip wandSound2; // "DM-CGS-47"
    public AudioClip waveEndSound; // "DM-CGS-45" "DM-CGS-26" "DM-CGS-23" "DM-CGS-18"
    public AudioClip healSound; // "02_Heal_02"
    public AudioClip dashSound; // "35_Miss_Evade_02"
    public AudioClip nightAmbientSound; // "Night_Loop_Stereo"
    public AudioClip dayAmbientSound; // "Forest_Birds_Loop_Stereo"

    private HashSet<AudioClip> soundEffects;

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

        // Wczytaj d�wi�ki
        soundEffects = new HashSet<AudioClip>
        {
            pauseSound,
            nightAmbientSound,
            dayAmbientSound,
            healSound,
            unpasueSound,
            wandSound2,
            wandSound1,
            waveEndSound,
            dashSound,
            playerDamageSound,
            playerDeathSound,
            lightningSound,
            walkSound,
            stupefySound,
            meteorRainSound,
            shieldSound,
            buttonClickSound,
            jumpSound
        };
    }

    private void Start()
    {
    }

    private void PlaySound(AudioClip soundName)
    {
        if (soundEffects.Contains(soundName))
        {
            playerSource.PlayOneShot(soundName);
        }
    }

    public void PlayPauseSound() => PlaySound(pauseSound);
    public void PlayMightAmbientSound() => PlaySound(unpasueSound);
    public void PlayDayAmbientSound() => PlaySound(dayAmbientSound);
    public void PlayHealSound() => PlaySound(healSound);
    public void PlayUnpauseSound() => PlaySound(unpasueSound);
    private void PlayWandSound2() => PlaySound(wandSound2);
    private void PlayWandSound1() => PlaySound(wandSound1);
    public void PlayWaveEndSound() => PlaySound(waveEndSound);
    public void PlayDashSound() => PlaySound(dashSound);
    public void PlayPlayerDamageSound() => PlaySound(playerDamageSound);
    public void PlayPlayerDeathSound() => PlaySound(playerDeathSound);
    public void PlayLightningSound() => PlaySound(lightningSound);
    public void PlayWalkSound() => PlaySound(walkSound);
    public void PlayStupefySound()
    {
        if (Random.Range(0, 1) <= 1) PlayWandSound1();
        else PlayWandSound2();
        PlaySound(stupefySound);
    }
    public void PlayMeteorRainSound() => PlaySound(meteorRainSound);
    public void PlayShieldSound() => PlaySound(shieldSound);
    public void PlayButtonClickSound() => PlaySound(buttonClickSound);
    public void PlayJumpSound() => PlaySound(jumpSound);


    public void SetVolume(float volume)
    {
        backgroundMusicSource.volume = volume;
        playerSource.volume = volume;
    }
}
