using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public float detectionRadius = 10f;
    List<GameObject> enemiesInRange = new List<GameObject>();

    public GameObject GetClosestEnemy(List<GameObject> excludedEnemies = null)
    {
        UpdateEnemiesInRange();

        if (enemiesInRange.Count > 0)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (GameObject closestEnemy in enemiesInRange)
            {
                if (excludedEnemies != null && excludedEnemies.Contains(closestEnemy))
                {
                    continue;
                }

                Vector3 directionToTarget = closestEnemy.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = closestEnemy;
                }
            }

            return bestTarget;
        }
        else
        {
            return null;
        }
    }

    public List<GameObject> GetEnemiesInRange()
    {
        UpdateEnemiesInRange();
        return enemiesInRange;
    }

    private void UpdateEnemiesInRange()
    {
        enemiesInRange.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                enemiesInRange.Add(hitCollider.gameObject);
            }
        }
    }
}
