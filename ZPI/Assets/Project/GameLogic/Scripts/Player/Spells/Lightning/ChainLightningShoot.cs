using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChainLightningShoot : MonoBehaviour
{
    [SerializeField]
    private float refreshRate = 0.01f;
    [SerializeField]
    [Range(1, 10)]
    private int maximumEnemiesInChain = 3;
    [SerializeField]
    private float delayBetweenEachChain = 0.3f;
    private EnemyDetector playerEnemyDetector;
    [SerializeField]
    private GameObject lineRendererPrefab;
    [SerializeField]
    private float spellDuration = 1f;
    [SerializeField]
    private GameObject _endOfWand;
    private LightningBoltScript lightningBoltScript;

    private bool shooting;
    private bool shot;
    private GameObject currentClosestEnemy;
    private List<GameObject> enemiesInChain = new List<GameObject>();

    private void Start()
    {
        playerEnemyDetector = GetComponent<EnemyDetector>();
        lightningBoltScript = GetComponent<LightningBoltScript>();
    }

    void Update()
    {
    }

    public void StartShooting()
    {
        shooting = true;
        AudioManager.instance.PlayLightningSound();

        if (playerEnemyDetector != null && lineRendererPrefab != null)
        {
            if (!shot)
            {
                shot = true;

                currentClosestEnemy = playerEnemyDetector.GetClosestEnemy(enemiesInChain);
                if (currentClosestEnemy != null)
                {
                    enemiesInChain.Add(currentClosestEnemy);
                    lightningBoltScript.StartObject = _endOfWand;
                    lightningBoltScript.EndObject = currentClosestEnemy;

                    if (maximumEnemiesInChain > 1)
                    {
                        StartCoroutine(ChainReaction(currentClosestEnemy));
                    }
                    StartCoroutine(StopShootingAfterDelay(spellDuration));
                }
                else
                {
                    StopShooting();
                }
            }
        }
    }

    IEnumerator StopShootingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopShooting();
    }

    IEnumerator ChainReaction(GameObject closestEnemy)
    {
        yield return new WaitForSeconds(delayBetweenEachChain);

        if (enemiesInChain.Count >= maximumEnemiesInChain || !shooting)
        {
            yield break;
        }

        if (closestEnemy == null)
        {
            yield break;
        }

        GameObject nextEnemy = closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy(enemiesInChain);
        if (nextEnemy != null)
        {
            enemiesInChain.Add(nextEnemy);
            LightningBoltScript lightning = closestEnemy.GetComponent<LightningBoltScript>();
            lightning.StartObject = closestEnemy;
            lightning.EndObject = nextEnemy;

            Enemy enemyComponent = nextEnemy.GetComponent<Enemy>();
            StartCoroutine(ChainReaction(nextEnemy));
        }
    }

    public void StopShooting()
    {
        shooting = false;
        shot = false;

        foreach (var enemy in enemiesInChain)
        {
            if (enemy != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(60, Vector3.zero);
                enemy.GetComponent<LightningBoltScript>().ResetGameObjects();
            }
        }

        lightningBoltScript.ResetGameObjects();
        enemiesInChain.Clear();
    }
}


