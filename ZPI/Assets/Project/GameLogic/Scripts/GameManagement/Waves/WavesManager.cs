using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    public List<EnemySpawner> EnemySpawners;
    public float delayBetweenWaves = 5f;
    public int upgradeWavesInterval = 3;
    private int spawnersCleared = 0;
    private int currentWave = 1;

    public UpgradeMenu upgradeMenu;

    void Start()
    {
        foreach (var spawner in EnemySpawners)
        {
            spawner.OnWaveCleared += OnSpawnerWaveCleared;
        }
    }

    private void OnSpawnerWaveCleared()
    {
        spawnersCleared++;
        if (spawnersCleared >= EnemySpawners.Count)
        {
            spawnersCleared = 0;
            currentWave++;

            if (currentWave % upgradeWavesInterval == 0)
            {
                ShowUpgradeMenu();
            }
            else
            {
                StartCoroutine(DelayedStartNextWave());
            }
        }
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
        upgradeMenu.Show();
        Cursor.lockState = CursorLockMode.Confined;
        upgradeMenu.OnUpgradeSelected += OnUpgradeSelected;
    }

    private void OnUpgradeSelected()
    {
        GameState.Instance.IsGamePaused = false;
        GameState.Instance.IsUpgrading = false;
        upgradeMenu.OnUpgradeSelected -= OnUpgradeSelected;
        StartCoroutine(DelayedStartNextWave());
    }
}
