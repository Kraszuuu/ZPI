using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    public List<EnemySpawner> EnemySpawners;
    public float delayBetweenWaves = 5f;
    public int upgradeWavesInterval = 3;
    private int _spawnersCleared = 0;
    private int _currentWave = 1;

    public UpgradeMenu upgradeMenu;
    public GameObject waveClearedText;
    public float waveClearedDisplayTime = 2f;
    public float intervalBeforeUpgradeMenuShow = 0.3f;

    public GameObject crosshair;

    void Start()
    {
        foreach (var spawner in EnemySpawners)
        {
            spawner.OnWaveCleared += OnSpawnerWaveCleared;
        }
    }

    public int GetCurrentWave()
    {
        return _currentWave;
    }


    private void OnSpawnerWaveCleared()
    {
        _spawnersCleared++;
        if (_spawnersCleared >= EnemySpawners.Count)
        {
            _spawnersCleared = 0;
            _currentWave++;
            AudioManager.instance.PlayWaveEndSound();

            if (_currentWave % upgradeWavesInterval == 0)
            {
                EnemiesStats.Instance.SetHealthData("Zombie", EnemiesStats.Instance.GetHealthData("Zombie") + 8f);
                EnemiesStats.Instance.SetDamageData("Zombie", EnemiesStats.Instance.GetDamageData("Zombie") + 2f);
                EnemiesStats.Instance.SetHealthData("Skeleton", EnemiesStats.Instance.GetHealthData("Skeleton") + 5f);
                EnemiesStats.Instance.SetDamageData("Skeleton", EnemiesStats.Instance.GetDamageData("Skeleton") + 1f);
                if (!IsEverySkillUnlocked())
                {
                    StartCoroutine(ShowWaveClearedAndUpgradeMenu());
                }
                else
                {
                    StartCoroutine(ShowWaveClearedAndStartNextWave());
                }
            }
            else
            {
                StartCoroutine(ShowWaveClearedAndStartNextWave());
            }
        }
    }

    private IEnumerator ShowWaveClearedAndStartNextWave()
    {
        yield return ShowWaveClearedMessage();
        yield return new WaitForSeconds(delayBetweenWaves);
        StartNextWave();
    }

    private IEnumerator ShowWaveClearedAndUpgradeMenu()
    {
        yield return ShowWaveClearedMessage();
        yield return new WaitForSeconds(intervalBeforeUpgradeMenuShow);
        ShowUpgradeMenu();
    }

    private IEnumerator ShowWaveClearedMessage()
    {
        waveClearedText.SetActive(true);
        crosshair.SetActive(false);
        yield return new WaitForSeconds(waveClearedDisplayTime);
        waveClearedText.gameObject.SetActive(false);
        crosshair.SetActive(true);
    }

    private IEnumerator DelayedStartNextWave()
    {
        yield return new WaitForSeconds(delayBetweenWaves);
        StartNextWave();
    }

    private void StartNextWave()
    {
        foreach (var spawner in EnemySpawners)
        {
            spawner.SpawnWave();
        }
    }

    private void ShowUpgradeMenu()
    {
        GameState.Instance.IsGamePaused = true;
        GameState.Instance.IsUpgrading = true;
        crosshair.SetActive(false);
        upgradeMenu.Show();
        Cursor.lockState = CursorLockMode.Confined;
        upgradeMenu.OnUpgradeSelected += OnUpgradeSelected;
    }

    private bool IsEverySkillUnlocked()
    {
        return upgradeMenu.playerSkills.IsEverySkillUnlocked();
    }

    private void OnUpgradeSelected()
    {
        GameState.Instance.IsGamePaused = false;
        GameState.Instance.IsUpgrading = false;
        crosshair.SetActive(true);
        upgradeMenu.OnUpgradeSelected -= OnUpgradeSelected;
        StartCoroutine(DelayedStartNextWave());
    }
}
