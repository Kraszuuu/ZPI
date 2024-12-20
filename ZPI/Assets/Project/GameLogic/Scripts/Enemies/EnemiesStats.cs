using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesStats : MonoBehaviour
{
    public static EnemiesStats Instance { get; private set; }

    private Dictionary<string, float> enemiesHealth = new();
    private Dictionary<string, float> enemiesDamage = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeStats();
    }

    private void InitializeStats()
    {
        enemiesDamage["Zombie"] = 6f;
        enemiesDamage["Skeleton"] = 8f;
        enemiesHealth["Zombie"] = 80f;
        enemiesHealth["Skeleton"] = 50f;
    }

    public float GetHealthData(string enemy)
    {
        if (enemiesHealth.TryGetValue(enemy, out float value))
        {
            return value;
        }
        Debug.LogWarning($"Health data for {enemy} not found!");
        return 0;
    }

    public float GetDamageData(string enemy)
    {
        if (enemiesDamage.TryGetValue(enemy, out float value))
        {
            return value;
        }
        Debug.LogWarning($"Damage data for {enemy} not found!");
        return 0;
    }

    public void SetHealthData(string enemy, float value)
    {
        if (enemiesHealth.ContainsKey(enemy))
        {
            enemiesHealth[enemy] = value;
        }
    }

    public void SetDamageData(string enemy, float value)
    {
        if (enemiesDamage.ContainsKey(enemy))
        {
            enemiesDamage[enemy] = value;
        }
    }
}
