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
    [SerializeField]
    private Transform playerFirePoint;
    [SerializeField]
    private EnemyDetector playerEnemyDetector;
    [SerializeField]
    private GameObject lineRendererPrefab;

    private bool shooting;
    private bool shot;
    private GameObject currentClosestEnemy;
    private List<GameObject> spawnedLineRenderers = new List<GameObject>();
    private List<GameObject> enemiesInChain = new List<GameObject>();

    void Update()
    {
        if (Input.GetKey(KeyCode.Y))
        {
            if (playerEnemyDetector.GetEnemiesInRange().Count > 0)
            {
                if (!shooting)
                {
                    StartShooting();
                }
            }
            else
            {
                StopShooting();
            }
        }
    }

    void StartShooting()
    {
        shooting = true;

        if (playerEnemyDetector != null && playerFirePoint != null && lineRendererPrefab != null)
        {
            if (!shot)
            {
                shot = true;

                currentClosestEnemy = playerEnemyDetector.GetClosestEnemy(enemiesInChain);
                if (currentClosestEnemy != null)
                {
                    enemiesInChain.Add(currentClosestEnemy);
                    NewLineRenderer(playerFirePoint, currentClosestEnemy.transform, true);

                    if (maximumEnemiesInChain > 1)
                    {
                        StartCoroutine(ChainReaction(currentClosestEnemy));
                    }

                    // Uruchomienie coroutine, aby po 3 sekundach wywo�a� StopShooting()
                    StartCoroutine(StopShootingAfterDelay(3f));
                }
            }
        }
    }

    IEnumerator StopShootingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopShooting();
    }

    void NewLineRenderer(Transform startPos, Transform endPos, bool fromPlayer = false)
    {
        GameObject lineR = Instantiate(lineRendererPrefab);
        spawnedLineRenderers.Add(lineR);
        StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos, fromPlayer));
    }

    IEnumerator UpdateLineRenderer(GameObject lineR, Transform startPos, Transform endPos, bool fromPlayer = false)
    {
        if (shooting && shot && lineR != null)
        {
            lineR.GetComponent<LineRendererController>().SetPosition(startPos, endPos);

            yield return new WaitForSeconds(refreshRate);

            if (fromPlayer)
            {
                StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos, true));
            }
            else
            {
                StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos));
            }
        }
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
            NewLineRenderer(closestEnemy.transform, nextEnemy.transform);

            Enemy enemyComponent = nextEnemy.GetComponent<Enemy>();
            StartCoroutine(ChainReaction(nextEnemy));
        }
    }

    void StopShooting()
    {
        shooting = false;
        shot = false;

        foreach (var enemy in enemiesInChain)
        {
            enemy.GetComponent<Enemy>().TakeDamage(60);
        }

        foreach (var lineRenderer in spawnedLineRenderers)
        {
            Destroy(lineRenderer);
        }

        spawnedLineRenderers.Clear();
        enemiesInChain.Clear();
    }
}


