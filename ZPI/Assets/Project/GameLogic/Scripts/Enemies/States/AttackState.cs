using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttackState : BaseState
{
    private float _moveTimer;
    private float _attackTimer;
    private float _stopDistanceRanged = 10f;
    private float _stopDistanceMelee = 2f;
    private PlayerHealth _playerHealth;
    private float _meleeDamage = 10f;

    private bool _attackScheduled = false; // Flaga, czy atak jest zaplanowany
    private float _attackDelayTimer = 0f; // Timer do odliczania opóźnienia

    private Vector3 _playerPosition;
    private float _distanceToPlayer;

    public override void Enter()
    {
        enemy.Agent.angularSpeed = 180f;
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
        _playerPosition = enemy.Player.transform.position;
        _distanceToPlayer = Vector3.Distance(enemy.transform.position, _playerPosition);

        if (enemy.CanSeePlayer())
        {
            DetectionManager.Instance.ReportPlayerDetected(_playerPosition);
        }

        if (DetectionManager.Instance.PlayerDetected) // Gracz widoczny
        {
            _moveTimer += Time.deltaTime;
            _attackTimer += Time.deltaTime;

            if (_attackTimer > enemy.fireRate && !_attackScheduled)
            {
                AttackPlayer();
            }

            // else if (_moveTimer > Random.Range(, 7))
            // {
            //     enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 3f));
            //     _moveTimer = 0;
            // }
        }
        else // Gracz zgubiony
        {
            // Przejście do stanu poszukiwań z ostatnią znaną pozycją gracza
            stateMachine.ChangeState(new SearchState());
        }

        // Obsługa zaplanowanego ataku
        if (_attackScheduled)
        {
            _attackDelayTimer -= Time.deltaTime;
            if (_attackDelayTimer <= 0)
            {
                // Sprawdzenie odległości w momencie trafienia
                if (_distanceToPlayer <= 3f)
                {
                    _playerHealth.TakeDamage(_meleeDamage);
                }

                _attackScheduled = false; // Resetuj planowanie ataku
                enemy.Agent.isStopped = false;
            }
        }
    }


    public void AttackPlayer()
    {
        if (Vector3.Distance(enemy.Agent.destination, _playerPosition) > 0.5f)
        {
            enemy.Agent.SetDestination(_playerPosition);
        }

        // Jeżeli można atakować i obecnie atak nie ma miejsca to próbuj atakować
        if (_attackTimer > enemy.fireRate && !_attackScheduled)
        {
            if (enemy.enemyType == EnemyType.Ranged)
            {
                if (_distanceToPlayer <= _stopDistanceRanged)
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
                    AudioManager.instance.PlayEnemyAttackSound(enemy.audioSource, enemy.enemyType);
                }
                else
                {
                    Debug.LogWarning("Prefab bullet not assigned!");
                }
            }
            else if (enemy.enemyType == EnemyType.Melee)
            {
                if (_distanceToPlayer > _stopDistanceMelee)
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

                    AudioManager.instance.PlayEnemyAttackSound(enemy.audioSource, enemy.enemyType);

                    stateMachine.SetAnimatorInteger("attackIndex", attackIndex);
                    stateMachine.SetAnimatorTrigger("attack");

                    _attackScheduled = true; // Aktywuj licznik czasu
                    _attackTimer = 0;
                }
            }
        }
    }

    private static float CalculateAnimationTime(int frames, int fps = 30)
    {
        return (float)frames / fps;
    }

}
