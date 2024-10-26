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

    private bool waveInProgress = false; // Zmienna kontrolna dla sprawdzenia aktywnej fali

    // Start is called before the first frame update
    void Start()
    {
        SpawnWave();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentEnemies.Count == 0 && !waveInProgress)
        {
            StartCoroutine(StartNextWave());
        }
    }

    void SpawnWave()
    {
        for (int i = 0; i < waves[currentWave].GetEnemySpawnList().Count; i++)
        {
            GameObject newSpawn = Instantiate(waves[currentWave].GetEnemySpawnList()[i], FindSpawnLoc(), Quaternion.identity);
            CurrentEnemies.Add(newSpawn);

            Enemy enemy = newSpawn.GetComponent<Enemy>();
            enemy.SetSpawner(this);
        }
    }

    IEnumerator StartNextWave()
    {
        waveInProgress = true; // Uniemo¿liwiamy kolejne wywo³ania StartNextWave
        yield return new WaitForSeconds(5);
        currentWave++;

        if (currentWave < waves.Count) // Upewniamy siê, ¿e nie wyjdziemy poza zakres listy fal
        {
            SpawnWave();
        }
        waveInProgress = false; // Reset zmiennej kontrolnej po zakoñczeniu fali
    }

    Vector3 FindSpawnLoc()
    {
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(2f, spawnRange);

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
