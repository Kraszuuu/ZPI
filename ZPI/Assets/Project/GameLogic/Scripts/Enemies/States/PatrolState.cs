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
            stateMachine.SetAnimatorBool("chase", true);
            stateMachine.ChangeState(new AttackState());
        }
        if (enemy.Agent.remainingDistance < 5f)
        {
            DetectionManager.Instance.ResetDetection();
            currentTarget = GetNewRandomDestination();
        }
    }

    private void PatrolCycle()
    {
        if (DetectionManager.Instance.PlayerDetected)
        {

            currentTarget = DetectionManager.Instance.LastKnownPlayerPosition;
            enemy.Agent.SetDestination(currentTarget);

        }
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
}
