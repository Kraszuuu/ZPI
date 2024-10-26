using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : BaseState
{
    private Vector3 currentTarget;
    private float patrolRadius = 10f;
    private float maxNavMeshDistance = 5f;

    public override void Enter()
    {
        currentTarget = GetNewRandomDestination();
    }

    public override void Exit()
    {
    }

    public override void Perform()
    {
        PatrolCycle();
        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new AttackState());
        }
    }

    private void PatrolCycle()
    {
        if (enemy.Agent.remainingDistance < 3f)
        {
            enemy.Agent.SetDestination(currentTarget);
            currentTarget = GetNewRandomDestination();
        }
    }

    private Vector3 GetNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += enemy.transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, maxNavMeshDistance, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        else
        {
            return GetNewRandomDestination();
        }
    }

    private void ChooseRandomPoint()
    {
        // Wybierz losowy punkt w promieniu widzenia
        Vector2 randomDirection = Random.insideUnitCircle * enemy.sightDistance;
        targetPosition = new Vector3(randomDirection.x + enemy.transform.position.x, enemy.transform.position.y, randomDirection.y + enemy.transform.position.z);

        // Ustaw nowy cel
        enemy.Agent.SetDestination(targetPosition);

        // Zresetuj timer oczekiwania
        waitTimer = 0;

        // Losowo wybierz czas oczekiwania na nowy punkt
        cycleTime = Random.Range(minCycleTime, maxCycleTime);
    }
}
