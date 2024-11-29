using UnityEngine;

public class AimState : BaseState
{
    private float aimDuration = 2f; // Czas celowania przed strzałem
    private float moveSidewaysTimer = 0f; // Timer do poruszania się na boki
    private float maxSidewaysMoveInterval = 3f; // Maksymalny czas, po którym łucznik zmienia kierunek ruchu
    private float attackCooldown = 3f; // Czas między kolejnymi strzałami
    private float attackTimer = 0f; // Aktualny licznik czasu do kolejnego strzału
    private Vector3 strafeDirection; // Kierunek ruchu na boki
    private float idealDistance = 10f; // Idealna odległość do gracza
    private Vector3 lastKnownPlayerPosition; // Ostatnia znana pozycja gracza

    public override void Enter()
    {
        enemy.Agent.updateRotation = false;
        stateMachine.SetAnimatorBool("aim", false); // Na początku stanu nie celuje
        lastKnownPlayerPosition = enemy.Player.transform.position; // Inicjalizacja ostatniej znanej pozycji gracza
        strafeDirection = Random.insideUnitCircle.normalized; // Losowanie kierunku poruszania się
        strafeDirection.y = 0;
    }

    public override void Exit()
    {
        enemy.Agent.updateRotation = true;
        stateMachine.SetAnimatorBool("aim", false); // Wyłączenie celowania przy wyjściu ze stanu
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer())
        {
            // Aktualizuj ostatnią znaną pozycję gracza, jeśli widoczny
            lastKnownPlayerPosition = enemy.Player.transform.position;
            AimAtPlayer();
        }
        else
        {
            // Jeśli gracz nie jest widoczny, celuj w ostatnią znaną pozycję
            AimAtLastKnownPosition();
        }

        // Sprawdź, czy można oddać strzał
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            if (!stateMachine.GetAnimatorBool("aim"))
            {
                // Ustaw celowanie i przygotuj strzał
                stateMachine.SetAnimatorBool("aim", true);
            }
            else
            {
                TriggerAttack();
            }
        }

        // Poruszaj się na boki
        MoveSideways();
    }

    private void AimAtPlayer()
    {
        Vector3 playerPosition = enemy.Player.transform.position;
        playerPosition.y = enemy.transform.position.y; // Upewnij się, że łucznik obraca się tylko w osi Y
        enemy.transform.LookAt(playerPosition);
    }

    private void AimAtLastKnownPosition()
    {
        Vector3 targetPosition = lastKnownPlayerPosition;
        targetPosition.y = enemy.transform.position.y; // Upewnij się, że łucznik obraca się tylko w osi Y
        enemy.transform.LookAt(targetPosition);
    }

    private void TriggerAttack()
    {
        // Wyzwól trigger ataku
        stateMachine.SetAnimatorTrigger("attack");

        // Po ataku ustaw aim na false
        stateMachine.SetAnimatorBool("aim", false);

        // Zresetuj timer ataku
        attackTimer = 0f;
    }

    private void MoveSideways()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, lastKnownPlayerPosition);

        if (distanceToPlayer < idealDistance - 2f)
        {
            Vector3 directionAwayFromPlayer = (enemy.transform.position - lastKnownPlayerPosition).normalized;
            enemy.Agent.SetDestination(enemy.transform.position + directionAwayFromPlayer * 2f); // Wycofaj się o 2 jednostki
        }
        else if (distanceToPlayer > idealDistance + 2f)
        {
            Vector3 directionTowardsPlayer = (lastKnownPlayerPosition - enemy.transform.position).normalized;
            enemy.Agent.SetDestination(enemy.transform.position + directionTowardsPlayer * 2f); // Zbliż się o 2 jednostki
        }
        else
        {
            // Ruch na boki
            moveSidewaysTimer += Time.deltaTime;
            if (moveSidewaysTimer > maxSidewaysMoveInterval)
            {
                // Zmień kierunek ruchu na boki
                strafeDirection = Random.insideUnitCircle.normalized;
                strafeDirection.y = 0;
                moveSidewaysTimer = 0f;
            }

            // Wyznacz nową pozycję w kierunku strafe
            Vector3 strafePosition = enemy.transform.position + strafeDirection * 2f;
            enemy.Agent.SetDestination(strafePosition);
        }
    }
}
