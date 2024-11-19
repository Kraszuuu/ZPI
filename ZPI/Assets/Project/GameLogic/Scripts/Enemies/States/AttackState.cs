using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttackState : BaseState
{
    private float _moveTimer;
    private float _losePlayerTimer;
    private float _attackTimer;
    private float _stopDistance = 5f;

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer()) //player can be seen
        {
            //lock the lose player timer and increment the move and shot timers
            _losePlayerTimer = 0;
            _moveTimer += Time.deltaTime;
            _attackTimer += Time.deltaTime;
            Vector3 playerPosition = enemy.Player.transform.position;
            playerPosition.y = enemy.transform.position.y;
            enemy.transform.LookAt(playerPosition);
            DetectionManager.Instance.ReportPlayerDetected(enemy.Player.transform.position);

            

            //move the enemy to a random position after a random time
            if (_attackTimer > enemy.fireRate)
            {
                AttackPlayer();
            }
            if (_moveTimer > Random.Range(3, 7))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                _moveTimer = 0;
            }
        }
        else //lost sight of player
        {
            _losePlayerTimer += Time.deltaTime;
            if (_losePlayerTimer > 2)
            {
                //change to the search state
                stateMachine.ChangeState(new PatrolState());
            }
        }
    }

    public void AttackPlayer()
    {
        // Oblicz odleg³oœæ miêdzy przeciwnikiem a graczem
        float distance = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position);

        if (enemy.enemyType == EnemyType.Ranged)
        {

            enemy.Agent.SetDestination(enemy.Player.transform.position);
            
           
            //store reference to the gun barrel
            Transform gunbarrel = enemy.gunBarrel;

            //instantiate a new bullt
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
            

            // Jeœli przeciwnik jest dalej ni¿ stopDistance, to siê porusza
            if (distance > _stopDistance)
            {
                Vector3 direction = (enemy.Player.transform.position - enemy.transform.position).normalized;
                enemy.transform.position += direction * Time.deltaTime;
                Vector3 playerPosition = enemy.Player.transform.position;
                playerPosition.y = enemy.transform.position.y;
                enemy.transform.LookAt(playerPosition);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
