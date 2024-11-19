using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveContent
    {
        [SerializeField]
        [NonReorderable]
        List<GameObject> enemySpawn;

        public List<GameObject> GetEnemySpawnList()
        {
            return enemySpawn;
        }
    }

    [SerializeField]
    [NonReorderable]
    List<WaveContent> waves;
    int currentWave = 0;
    float spawnRange = 6f;
    public List<GameObject> CurrentEnemies;

    public event Action OnWaveCleared;

    private bool waveInProgress = false;

    void Start()
    {
        SpawnWave();
    }

    void Update()
    {
        CurrentEnemies.RemoveAll(enemy => enemy == null);

        if (CurrentEnemies.Count == 0 && waveInProgress)
        {
            waveInProgress = false;
            OnWaveCleared?.Invoke();
        }
    }

    public void SpawnWave()
    {
        if (currentWave < waves.Count)
        {
            waveInProgress = true;
            for (int i = 0; i < waves[currentWave].GetEnemySpawnList().Count; i++)
            {
                GameObject newSpawn = Instantiate(waves[currentWave].GetEnemySpawnList()[i], FindSpawnLoc(), Quaternion.identity);
                CurrentEnemies.Add(newSpawn);
            }
            currentWave++;
        }
        else
        {
            waveInProgress = true;
            for (int i = 0; i < waves[currentWave - 1].GetEnemySpawnList().Count; i++)
            {
                GameObject newSpawn = Instantiate(waves[currentWave-1].GetEnemySpawnList()[i], FindSpawnLoc(), Quaternion.identity);
                CurrentEnemies.Add(newSpawn);
            }
        }
    }

    Vector3 FindSpawnLoc()
    {
        float angle = UnityEngine.Random.Range(0f, 360f);
        float distance = UnityEngine.Random.Range(2f, spawnRange);

        float xLoc = Mathf.Cos(angle * Mathf.Deg2Rad) * distance + transform.position.x;
        float zLoc = Mathf.Sin(angle * Mathf.Deg2Rad) * distance + transform.position.z;
        float yLoc = transform.position.y;

        Vector3 spawnPos = new Vector3(xLoc, yLoc, zLoc);

        if (Physics.Raycast(spawnPos, Vector3.down, 5))
        {
            return spawnPos;
        }
        else
        {
            return FindSpawnLoc();
        }
    }
}
