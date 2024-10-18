using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shotTimer;
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
            shotTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Player.transform);
            //move the enemy to a random position after a random time
            if (shotTimer > enemy.fireRate)
            {
                Shoot();
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

    public void Shoot()
    {
        //store reference to the gun barrel
        Transform gunbarrel = enemy.gunBarrel;

        //instantiate a new bullt
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunbarrel.position, enemy.transform.rotation);
        //calculate the direction to the player
        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.transform.position).normalized;
        //add force rigidbody of the bullet
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-2f, 2f), Vector3.up) * shootDirection * 40;
        shotTimer = 0;
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
