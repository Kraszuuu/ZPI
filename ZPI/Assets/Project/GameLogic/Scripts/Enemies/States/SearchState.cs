using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : BaseState
{
    private Vector3 _searchPosition;
    private float _searchTimer = 0f;
    private float _searchDuration = 8f; // Czas poszukiwania
    private float _searchRadius = 10f;
    private float _initialRandomOffsetRange = 4f; // Losowe przesunięcie przy obliczaniu początkowej pozycji
    private bool isWaitingForTransition = false; // Flaga blokująca inne operacje


    public SearchState()
    {
        // Dodanie losowego przesunięcia do ostatniej znanej pozycji
        _searchPosition = DetectionManager.Instance.LastKnownPlayerPosition + new Vector3(
            Random.Range(-_initialRandomOffsetRange, _initialRandomOffsetRange),
            0,
            Random.Range(-_initialRandomOffsetRange, _initialRandomOffsetRange)
        );
    }

    public override void Enter()
    {
        // Ustaw przeciwnika, aby przeszedł w kierunku zmodyfikowanej pozycji
        _searchTimer = 0f;
        enemy.Agent.SetDestination(_searchPosition);
    }

    public override void Exit()
    {
        isWaitingForTransition = false;
    }

    public override void Perform()
    {
        if (isWaitingForTransition)
        {
            return; // Zablokuj wykonywanie innych operacji, jeśli oczekujemy na przejście
        }

        if (DetectionManager.Instance.PlayerDetected)
        {
            float randomDelay = Random.Range(0.5f, 2f); // Losowe opóźnienie od 0.5 do 2 sekund
            enemy.StartCoroutine(TransitionToAttackStateWithDelay(randomDelay));
            return;
        }

        // Jeśli gracz zostanie zauważony podczas poszukiwań, przejdź z powrotem do stanu ataku
        if (enemy.CanSeePlayer())
        {
            DetectionManager.Instance.ReportPlayerDetected(enemy.Player.transform.position);
            stateMachine.SetAnimatorTrigger("scream");
            stateMachine.ChangeState(new AttackState());
            return;
        }

        _searchTimer += Time.deltaTime;

        if (_searchTimer >= _searchDuration)
        {
            // Jeśli czas poszukiwania minął, wróć do stanu patrolu
            stateMachine.SetAnimatorBool("chase", false);
            enemy.Agent.angularSpeed = 120f;
            stateMachine.ChangeState(new PatrolState());
            return;
        }

        // Jeśli dotarł do centrum poszukiwań, rozpocznij przeszukiwanie obszaru
        if (Vector3.Distance(enemy.transform.position, _searchPosition) < 3f)
        {
            // Wybierz losowy punkt w pobliżu i poruszaj się w tym kierunku
            if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance)
            {
                Vector3 randomDirection = Random.insideUnitSphere * _searchRadius;
                randomDirection.y = 0;
                Vector3 searchPoint = _searchPosition + randomDirection;

                if (NavMesh.SamplePosition(searchPoint, out NavMeshHit hit, _searchRadius, NavMesh.AllAreas))
                {
                    enemy.Agent.SetDestination(hit.position);
                }
            }
        }
    }

    private IEnumerator TransitionToAttackStateWithDelay(float delay)
    {
        isWaitingForTransition = true; // Ustaw flagę na czas oczekiwania
        yield return new WaitForSeconds(delay);
        TransitionToAttackState();
    }

    private void TransitionToAttackState()
    {
        stateMachine.SetAnimatorTrigger("scream");
        stateMachine.ChangeState(new AttackState());
        isWaitingForTransition = false; // Zresetuj flagę po zakończeniu przejścia
    }
}
