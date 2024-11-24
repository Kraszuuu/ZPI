using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttackState : BaseState
{
    private float _moveTimer;
    private float _losePlayerTimer;
    private float _attackTimer;
    private float _stopDistanceRanged = 10f;
    private float _stopDistanceMelee = 2f;
    private PlayerHealth _playerHealth;
    private float _meleeDamage = 10f;

    private bool _attackScheduled = false; // Flaga, czy atak jest zaplanowany
    private float _attackDelayTimer = 0f; // Timer do odliczania opóźnienia

    public override void Enter()
    {
        if (enemy.enemyType == EnemyType.Melee)
        {
            _playerHealth = enemy.Player.GetComponent<PlayerHealth>();
        }
    }

    public override void Exit()
    {

    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer()) // Gracz widoczny
        {
            _losePlayerTimer = 0;
            _moveTimer += Time.deltaTime;
            _attackTimer += Time.deltaTime;

            Vector3 playerPosition = enemy.Player.transform.position;
            playerPosition.y = enemy.transform.position.y;

            // enemy.transform.LookAt(playerPosition);
            DetectionManager.Instance.ReportPlayerDetected(enemy.Player.transform.position);

            if (_attackTimer > enemy.fireRate && !_attackScheduled)
            {
                AttackPlayer();
            }

            if (_moveTimer > Random.Range(3, 7))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                _moveTimer = 0;
            }
        }
        else // Gracz zgubiony
        {
            _losePlayerTimer += Time.deltaTime;
            if (_losePlayerTimer > 2)
            {
                // Przejście do stanu poszukiwań z ostatnią znaną pozycją gracza
                stateMachine.ChangeState(new SearchState(enemy.Player.transform.position));
            }
        }

        // Obsługa zaplanowanego ataku
        if (_attackScheduled)
        {
            _attackDelayTimer -= Time.deltaTime;
            if (_attackDelayTimer <= 0)
            {
                // Sprawdzenie odległości w momencie trafienia
                float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position);
                if (distanceToPlayer <= 3f)
                {
                    _playerHealth.TakeDamage(_meleeDamage);
                }

                _attackScheduled = false; // Resetuj planowanie ataku
            }
        }
    }


    public void AttackPlayer()
    {
        float distance = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position);
        enemy.Agent.SetDestination(enemy.Player.transform.position);

        if (enemy.enemyType == EnemyType.Ranged)
        {
            if (distance <= _stopDistanceRanged)
            {
                enemy.Agent.isStopped = true;
            }
            else
            {
                enemy.Agent.isStopped = false;
            }

            Transform gunbarrel = enemy.gunBarrel;

            if (enemy.bulletPrefab != null)
            {
                GameObject bullet = GameObject.Instantiate(enemy.bulletPrefab, gunbarrel.position, enemy.transform.rotation);
                //calculate the direction to the player
                Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.transform.position).normalized;
                //add force rigidbody of the bullet
                bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-2f, 2f), Vector3.up) * shootDirection * 40;
                _attackTimer = 0;
            }
            else
            {
                Debug.LogWarning("Prefab bullet not assigned!");
            }
        }
        else if (enemy.enemyType == EnemyType.Melee)
        {
            if (distance > _stopDistanceMelee)
            {
                enemy.Agent.isStopped = false;
            }
            else
            {
                enemy.Agent.isStopped = true;
                int attackIndex = Random.Range(0, 6);

                // each attack index has different attack animation and time when it hits, so we need to set different cases that will set delay when attack hits
                switch (attackIndex)
                {
                    case 0:
                        _attackDelayTimer = CalculateAnimationTime(40);
                        break;
                    case 1:
                        _attackDelayTimer = CalculateAnimationTime(40);
                        break;
                    case 2:
                        _attackDelayTimer = CalculateAnimationTime(20);
                        break;
                    case 3:
                        _attackDelayTimer = CalculateAnimationTime(38);
                        break;
                    case 4:
                        _attackDelayTimer = CalculateAnimationTime(24);
                        break;
                    case 5:
                        _attackDelayTimer = CalculateAnimationTime(33);
                        break;
                }

                stateMachine.SetAnimatorInteger("attackIndex", attackIndex);
                stateMachine.SetAnimatorTrigger("attack");

                _attackScheduled = true; // Aktywuj licznik czasu
                _attackTimer = 0;
            }
        }
    }

    private static float CalculateAnimationTime(int frames, int fps = 30)
    {
        return (float)frames / fps;
    }

}
