using UnityEngine;

public class AimState : BaseState
{
    private float moveSidewaysTimer = 0f; // Timer do poruszania się na boki
    private float maxSidewaysMoveInterval = 3f; // Maksymalny czas, po którym łucznik zmienia kierunek ruchu
    private float attackCooldown = 3f; // Czas między kolejnymi strzałami
    private float attackTimer = 3f;
    private float aimDuration = 3f;
    private float aimDurationTimer = 0f;
    private Vector3 strafeDirection; // Kierunek ruchu na boki
    private float idealDistance = 10f; // Idealna odległość do gracza

    public override void Enter()
    {
        stateMachine.SetAnimatorBool("aim", true); // Na początku stanu nie celuje
        enemy.Agent.updateRotation = false;
        strafeDirection = Random.insideUnitCircle.normalized; // Losowanie kierunku poruszania się
        strafeDirection.y = 0;
    }

    public override void Exit()
    {
        stateMachine.SetAnimatorBool("aim", false); // Wyłączenie celowania przy wyjściu ze stanu
        enemy.Agent.updateRotation = true;
    }

    public override void Perform()
    {
        if (DetectionManager.Instance.PlayerDetected)
        {
            if (enemy.CanSeePlayer())
            {
                DetectionManager.Instance.ReportPlayerDetected(enemy.Player.transform.position);
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
                    aimDurationTimer += Time.deltaTime;
                    if (aimDurationTimer > aimDuration)
                    {
                        TriggerAttack();
                    }
                }
            }

            // Poruszaj się na boki
            MoveSideways();
        }
        else
        {
            if (stateMachine.GetAnimatorBool("aim"))
            {
                TriggerAttack();
            }
            stateMachine.ChangeState(new SearchState());
        }

    }

    private void AimAtPlayer()
    {
        Vector3 playerPosition = enemy.Player.transform.position;
        playerPosition.y = enemy.transform.position.y; // Upewnij się, że łucznik obraca się tylko w osi Y
        enemy.transform.LookAt(playerPosition);
    }

    private void AimAtLastKnownPosition()
    {
        Vector3 targetPosition = DetectionManager.Instance.LastKnownPlayerPosition;
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
        aimDurationTimer = 0f;
    }

    private void MoveSideways()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position);

        // Dostosuj ruch do odległości od gracza
        Vector3 direction;
        if (distanceToPlayer < idealDistance)
        {
            // Jeśli zbyt blisko, poruszaj się na boki i lekko oddalaj
            direction = (enemy.transform.position - enemy.Player.transform.position).normalized + strafeDirection;
        }
        else
        {
            // Jeśli zbyt daleko, poruszaj się na boki i lekko zbliżaj
            direction = (enemy.Player.transform.position - enemy.transform.position).normalized + strafeDirection;
        }

        // Upewnij się, że kierunek ruchu jest znormalizowany
        direction = direction.normalized;

        // Aktualizuj cel ruchu
        Vector3 targetPosition = enemy.transform.position + direction * 2f;
        enemy.Agent.SetDestination(targetPosition);

        // Aktualizuj strafe co jakiś czas
        moveSidewaysTimer += Time.deltaTime;
        if (moveSidewaysTimer > maxSidewaysMoveInterval)
        {
            // Zmień kierunek ruchu na boki
            strafeDirection = Random.insideUnitCircle.normalized;
            strafeDirection.y = 0; // Zapewnij ruch w płaszczyźnie poziomej
            moveSidewaysTimer = 0f;
        }
    }
}
