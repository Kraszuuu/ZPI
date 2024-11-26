using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource _playerSource;

    [Header("Player Clips")]
    public AudioClip jumpSound; //"51_Flee_02"  "45_landing_01" "30_Jump_03"
    public AudioClip walkSound; // "03_Step_grass_03"
    public AudioClip playerDamageSound; // "39_Block_03" "08_Bite_04"
    public AudioClip playerDeathSound; // "DM-CGS-178" "male-death-sound"
    public AudioClip healSound; // "02_Heal_02"
    public AudioClip dashSound; // "35_Miss_Evade_02"

    [Header("Spells Clips")]
    public AudioClip wandSound1; // "DM-CGS-46"
    public AudioClip wandSound2; // "DM-CGS-47"
    public AudioClip lightningSound; // "18_Thunder_02"
    public AudioClip stupefySound; // "77_flesh_02" "22_Slash_04" "56_Attack_03"
    public AudioClip meteorRainSound; // "dropping-rocks-5996"
    public AudioClip shieldSound; // "21_Debuff_01" "39_Absorb_04"
    public AudioClip fireballSound;
    public AudioClip fireballExplosionSound;

    [Header("Zombie Clips")]
    public AudioClip zombieWalkSound;
    public AudioClip zombieDamageSound;
    public AudioClip zombieDeathSound;
    public AudioClip zombieAttackSound;
    public AudioClip zombieAmbientSound;
    public AudioClip zombieEnemySpotted;

    [Header("Skeleton Clips")]
    public AudioClip skeletonWalkSound;
    public AudioClip skeletonDamageSound;
    public AudioClip skeletonDeathSound;
    public AudioClip skeletonAttackSound;
    public AudioClip skeletonAmbientSound;
    public AudioClip skeletonArrowInAir;
    public AudioClip skeletonArrowHit;

    [Header("Other Clips")]
    public AudioClip buttonClickSound; // "ButtonClick"
    public AudioClip pauseSound; //"001_Hoover_01" "092_Pause_04"
    public AudioClip unpasueSound; // "098_Unpause_04"
    public AudioClip waveEndSound; // "DM-CGS-45" "DM-CGS-26" "DM-CGS-23" "DM-CGS-18"
    public AudioClip nightAmbientSound; // "Night_Loop_Stereo"
    public AudioClip dayAmbientSound; // "Forest_Birds_Loop_Stereo"
    public AudioClip exampleSound;

    //[Header("Volume Settings")]
    //[Range(0, 1)] private float _masterVolume;
    //[SerializeField] private Slider _slider;

    private HashSet<AudioClip> soundEffects;

    private void Start()
    {
        _playerSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
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
            jumpSound,
            fireballSound,
            zombieWalkSound,
            zombieDamageSound,
            zombieDeathSound,
            zombieAttackSound,
            zombieAmbientSound,
            skeletonWalkSound,
            skeletonDamageSound,
            skeletonDeathSound,
            skeletonAttackSound,
            skeletonAmbientSound,
            exampleSound,
            fireballExplosionSound,
            zombieEnemySpotted,
            skeletonArrowInAir,
            skeletonArrowHit
        };
    }

    private void PlaySoundFromPlayer(AudioClip soundName)
    {
        if (soundEffects.Contains(soundName))
        {
            //_playerSource.volume = _masterVolume;
            _playerSource.PlayOneShot(soundName);
        }
    }
    private void PlaySoundFromSource(AudioClip soundName, AudioSource audioSource)
    {
        if (soundEffects.Contains(soundName))
        {
            //audioSource.volume = _masterVolume;
            audioSource.PlayOneShot(soundName);
        }
    }

    // Player
    public void PlayHealSound() => PlaySoundFromPlayer(healSound);
    public void PlayDashSound() => PlaySoundFromPlayer(dashSound);
    public void PlayPlayerDamageSound() => PlaySoundFromPlayer(playerDamageSound);
    public void PlayPlayerDeathSound() => PlaySoundFromPlayer(playerDeathSound);
    public void PlayWalkSound() => PlaySoundFromPlayer(walkSound);
    public void PlayJumpSound() => PlaySoundFromPlayer(jumpSound);

    // Spells
    private void PlayWandSound2() => PlaySoundFromPlayer(wandSound2);
    private void PlayWandSound1() => PlaySoundFromPlayer(wandSound1);
    public void PlayStupefySound()
    {
        if (Random.Range(0, 1) < 1) PlayWandSound1();
        else PlayWandSound2();
        PlaySoundFromPlayer(stupefySound);
    }
    public void PlayLightningSound() => PlaySoundFromPlayer(lightningSound);
    public void PlayMeteorRainSound(AudioSource audioSource) => PlaySoundFromSource(meteorRainSound, audioSource);
    public void PlayShieldSound() => PlaySoundFromPlayer(shieldSound);
    public void PlayFireballSound(AudioSource audioSource) => PlaySoundFromSource(fireballSound, audioSource);

    public void PlayFireballExplosionSound(AudioSource audioSource) => PlaySoundFromSource(fireballExplosionSound, audioSource);

    // Enemies
    public void PlayEnemyWalkSound(AudioSource audioSource, EnemyType enemyType)
    {
        if (enemyType == EnemyType.Melee) PlayZombieWalkSound(audioSource);
        else if (enemyType == EnemyType.Ranged) PlaySkeletonWalkSound(audioSource);
        else Debug.LogError("Unknown EnemyType in AudioManager");
    } // unable to be used
    public void PlayEnemyDamageSound(AudioSource audioSource, EnemyType enemyType)
    {
        if (enemyType == EnemyType.Melee) PlayZombieDamageSound(audioSource);
        else if (enemyType == EnemyType.Ranged) PlaySkeletonDamageSound(audioSource);
        else Debug.LogError("Unknown EnemyType in AudioManager");
    }
    public void PlayEnemyDeathSound(AudioSource audioSource, EnemyType enemyType)
    {
        if (enemyType == EnemyType.Melee) PlayZombieDeathSound(audioSource);
        else if (enemyType == EnemyType.Ranged) PlaySkeletonDeathSound(audioSource);
        else Debug.LogError("Unknown EnemyType in AudioManager");
    }
    public void PlayEnemyAttackSound(AudioSource audioSource, EnemyType enemyType)
    {
        if (enemyType == EnemyType.Melee) PlayZombieAttackSound(audioSource);
        else if (enemyType == EnemyType.Ranged) PlaySkeletonAttackSound(audioSource);
        else Debug.LogError("Unknown EnemyType in AudioManager");
    }
    public void PlayEnemyAmbientSound(AudioSource audioSource, EnemyType enemyType)
    {
        if (enemyType == EnemyType.Melee) PlayZombieAmbientSound(audioSource);
        else if (enemyType == EnemyType.Ranged) PlaySkeletonAmbientSound(audioSource);
        else Debug.LogError("Unknown EnemyType in AudioManager");
    }


    // Zombie
    private void PlayZombieWalkSound(AudioSource audioSource) => PlaySoundFromSource(zombieWalkSound, audioSource);
    private void PlayZombieDamageSound(AudioSource audioSource) => PlaySoundFromSource(zombieDamageSound, audioSource);
    private void PlayZombieDeathSound(AudioSource audioSource) => PlaySoundFromSource(zombieDeathSound, audioSource);
    private void PlayZombieAttackSound(AudioSource audioSource) => PlaySoundFromSource(zombieAttackSound, audioSource);
    private void PlayZombieAmbientSound(AudioSource audioSource) => PlaySoundFromSource(zombieAmbientSound, audioSource);
    public void PlayZombieEnemySpottedSound(AudioSource audioSource) => PlaySoundFromSource(zombieEnemySpotted, audioSource);
    // Skeleton
    private void PlaySkeletonWalkSound(AudioSource audioSource) => PlaySoundFromSource(skeletonWalkSound, audioSource);
    private void PlaySkeletonDamageSound(AudioSource audioSource) => PlaySoundFromSource(skeletonDamageSound, audioSource);
    private void PlaySkeletonDeathSound(AudioSource audioSource) => PlaySoundFromSource(skeletonDeathSound, audioSource);
    private void PlaySkeletonAttackSound(AudioSource audioSource) => PlaySoundFromSource(skeletonAttackSound, audioSource);
    private void PlaySkeletonAmbientSound(AudioSource audioSource) => PlaySoundFromSource(skeletonAmbientSound, audioSource);
    public void PlayArrowInAirSound(AudioSource audioSource) => PlaySoundFromSource(skeletonArrowInAir, audioSource);
    public void PlayArrowHitSound(AudioSource audioSource) => PlaySoundFromSource(skeletonArrowHit, audioSource);

    // Other
    public void PlayPauseSound() => PlaySoundFromPlayer(pauseSound);
    public void PlayMightAmbientSound() => PlaySoundFromPlayer(unpasueSound);
    public void PlayDayAmbientSound() => PlaySoundFromPlayer(dayAmbientSound);
    public void PlayUnpauseSound() => PlaySoundFromPlayer(unpasueSound);
    public void PlayWaveEndSound() => PlaySoundFromPlayer(waveEndSound);
    public void PlayButtonClickSound() => PlaySoundFromPlayer(buttonClickSound);
}
