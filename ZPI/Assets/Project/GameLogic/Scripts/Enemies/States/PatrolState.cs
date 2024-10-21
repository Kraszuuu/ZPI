using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PatrolState : BaseState
{
    private Vector3 targetPosition;
    public int waypointIndex;
    public float waitTimer;
    private float cycleTime; // Czas cyklu patrolowania, inicjowany w Enter()
    public float minCycleTime = 2f; // Minimalny czas cyklu
    public float maxCycleTime = 8f; // Maksymalny czas cyklu
    public LayerMask wallLayer; // Warstwa dla œcian

    public override void Enter()
    {
        // Inicjalizuj cykl czasowy losowo w ustalonym zakresie
        cycleTime = Random.Range(minCycleTime, maxCycleTime);
        ChooseRandomPoint(); // Wybierz losowy punkt na pocz¹tku patrolu
        waitTimer = 0; // Zresetuj timer
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

    public void PatrolCycle()
    {
        // Jeœli jest w ruchu, zresetuj timer
        if (waitTimer < cycleTime)
        {
            waitTimer += Time.deltaTime; // Zwiêksz timer czekania
        }
        else
        {
            // Jeœli czas oczekiwania min¹³, wybierz nowy cel
            ChooseRandomPoint();
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
