using UnityEngine;
using UnityEngine.AI;

public class SearchState : BaseState
{
    private Vector3 _searchCenter;
    private float _searchTimer = 0f;
    private float _searchDuration = 5f; // Czas poszukiwania
    private float _searchRadius = 10f;
    private float _initialRandomOffsetRange = 4f; // Losowe przesunięcie przy obliczaniu początkowej pozycji

    public SearchState(Vector3 lastKnownPosition)
    {
        // Dodanie losowego przesunięcia do ostatniej znanej pozycji
        _searchCenter = lastKnownPosition + new Vector3(
            Random.Range(-_initialRandomOffsetRange, _initialRandomOffsetRange),
            0,
            Random.Range(-_initialRandomOffsetRange, _initialRandomOffsetRange)
        );
    }

    public override void Enter()
    {
        // Ustaw przeciwnika, aby przeszedł w kierunku zmodyfikowanej pozycji
        enemy.Agent.SetDestination(_searchCenter);
    }

    public override void Exit()
    {
        // Zatrzymanie agenta na wyjściu ze stanu
        enemy.Agent.isStopped = true;
    }

    public override void Perform()
    {
        // Jeśli dotarł do centrum poszukiwań, rozpocznij przeszukiwanie obszaru
        if (Vector3.Distance(enemy.transform.position, _searchCenter) < 1f)
        {
            _searchTimer += Time.deltaTime;

            if (_searchTimer >= _searchDuration)
            {
                // Jeśli czas poszukiwania minął, wróć do stanu patrolu
                stateMachine.SetAnimatorBool("chase", false);
                stateMachine.ChangeState(new PatrolState());
                return;
            }

            // Wybierz losowy punkt w pobliżu i poruszaj się w tym kierunku
            if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance)
            {
                Vector3 randomDirection = Random.insideUnitSphere * _searchRadius;
                randomDirection.y = 0;
                Vector3 searchPoint = _searchCenter + randomDirection;

                if (NavMesh.SamplePosition(searchPoint, out NavMeshHit hit, _searchRadius, NavMesh.AllAreas))
                {
                    enemy.Agent.SetDestination(hit.position);
                }
            }
        }

        // Jeśli gracz zostanie zauważony podczas poszukiwań, przejdź z powrotem do stanu ataku
        if (enemy.CanSeePlayer())
        {
            stateMachine.SetAnimatorTrigger("scream");
            stateMachine.ChangeState(new AttackState());
        }
    }
}
