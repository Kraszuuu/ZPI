using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : BaseState
{
    private const float _patrolRadius = 10f;
    private const float _maxNavMeshDistance = 6f;
    private const float _closeDistanceThreshold = 2f; // Minimalna odległość do uznania celu za osiągnięty
    private const int _maxAttempts = 10; // Maksymalna liczba prób wygenerowania celu

    private bool isWaitingForTransition = false; // Flaga blokująca inne operacje

    public override void Enter()
    {
        enemy.Agent.angularSpeed = 120f;
        SetNewRandomDestination();
    }

    public override void Exit()
    {
        isWaitingForTransition = false; // Zresetuj flagę podczas wyjścia
    }

    public override void Perform()
    {
        if (isWaitingForTransition)
        {
            return; // Zablokuj wykonywanie innych operacji, jeśli oczekujemy na przejście
        }

        if (HandlePlayerDetection())
        {
            return; // Jeśli gracz został wykryty, zakończ metodę
        }

        if (enemy.Agent.remainingDistance < _closeDistanceThreshold)
        {
            SetNewRandomDestination();
        }
    }

    private bool HandlePlayerDetection()
    {
        // Jeśli gracz został zgłoszony przez innego przeciwnika
        if (DetectionManager.Instance.PlayerDetected)
        {
            float randomDelay = Random.Range(0.5f, 2f); // Losowe opóźnienie od 0.5 do 2 sekund
            enemy.StartCoroutine(TransitionToAttackStateWithDelay(randomDelay));
            return true;
        }

        // Jeśli przeciwnik sam wykrywa gracza
        if (enemy.CanSeePlayer())
        {
            DetectionManager.Instance.ReportPlayerDetected(enemy.Player.transform.position);
            TransitionToAttackState();
            return true;
        }

        return false;
    }

    private IEnumerator TransitionToAttackStateWithDelay(float delay)
    {
        isWaitingForTransition = true; // Ustaw flagę na czas oczekiwania
        yield return new WaitForSeconds(delay);
        TransitionToAttackState();
    }

    private void TransitionToAttackState()
    {
        stateMachine.SetAnimatorBool("chase", true);
        enemy.Agent.angularSpeed = 180f;
        stateMachine.ChangeState(new AttackState());
        isWaitingForTransition = false; // Zresetuj flagę po zakończeniu przejścia
    }

    private void SetNewRandomDestination()
    {
        Vector3 randomDirection;
        NavMeshHit navHit;

        for (int attempts = 0; attempts < _maxAttempts; attempts++)
        {
            randomDirection = Random.insideUnitSphere * _patrolRadius + enemy.transform.position;

            if (NavMesh.SamplePosition(randomDirection, out navHit, _maxNavMeshDistance, NavMesh.AllAreas)
                && Vector3.Distance(enemy.transform.position, navHit.position) >= _closeDistanceThreshold)
            {
                enemy.Agent.SetDestination(navHit.position);
                return;
            }
        }

        // Jeśli nie udało się znaleźć odpowiedniego punktu, ustaw bieżącą pozycję jako cel
        enemy.Agent.SetDestination(enemy.transform.position);
    }
}
