using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float attackTimer;
    private float stopDistance = 1.5f;

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
            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Player.transform);
            //move the enemy to a random position after a random time
            if (attackTimer > enemy.fireRate)
            {
                AttackPlayer();
            }
            if (moveTimer > Random.Range(3, 7))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                moveTimer = 0;
            }
        }
        else //lost sight of player
        {
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > 8)
            {
                //change to the search state
                stateMachine.ChangeState(new PatrolState());
            }
        }
    }

    public void AttackPlayer()
    {
        if (enemy.enemyType == EnemyType.Ranged)
        {
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
                attackTimer = 0;
            }
            else
            {
                Debug.LogWarning("Prefab bullet not assigned!");
            }
        }
        else if (enemy.enemyType == EnemyType.Melee)
        {
            // Oblicz odleg³oœæ miêdzy przeciwnikiem a graczem
            float distance = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position);

            // Jeœli przeciwnik jest dalej ni¿ stopDistance, to siê porusza
            if (distance > stopDistance)
            {
                Vector3 direction = (enemy.Player.transform.position - enemy.transform.position).normalized;
                enemy.transform.position += direction * Time.deltaTime;
                enemy.transform.LookAt(enemy.Player.transform.position);
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
