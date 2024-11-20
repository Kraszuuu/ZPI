using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;

    public NavMeshAgent Agent { get => agent; }
    public GameObject Player { get => player; }

    public Path path;

    [Header("Speed")]
    public float speed;

    [Header("Enemy class")]
    public EnemyType enemyType;

    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight;

    [Header("Weapon Values")]
    public Transform gunBarrel;
    public GameObject bulletPrefab;
    [Range(0.1f, 10)]
    public float fireRate;

    [Header("Health Values")]
    public int maxHealth = 100;
    private int currentHealth;

    [SerializeField]
    private string currentState;

    private EnemySpawner spawner;
    private DamagePopupGenerator _damagePopupGenerator;

    private PlayerHealth healthBar;
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();
        _damagePopupGenerator = GetComponentInChildren<DamagePopupGenerator>();
        player = GameObject.FindGameObjectWithTag("Player");

        currentHealth = maxHealth;
        healthBar = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        //is the player close enough to be seen?
        if (Vector3.Distance(transform.position, player.transform.position) >= sightDistance) return false;

        //is the player in enemy's field of view
        Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
        float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
        if (angleToPlayer < -fieldOfView || angleToPlayer > fieldOfView) return false;

        //is enemy looking at the player (?)
        Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
        RaycastHit hitInfo = new RaycastHit();

        int layerToIgnore = LayerMask.NameToLayer("Shield");
        int layerMask = ~(1 << layerToIgnore);

        if (!Physics.Raycast(ray, out hitInfo, sightDistance, layerMask) || hitInfo.transform.gameObject != player) return false;

        Debug.DrawRay(ray.origin, ray.direction * sightDistance, Color.yellow);

        return true;
    }

    public void TakeDamage(int damage)
    {
        DetectionManager.Instance.ReportPlayerDetected(player.transform.position);
        currentHealth -= damage;
        healthBar.TakeDamage(damage);
        Vector3 randomness = new Vector3(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
        _damagePopupGenerator.CreatePopup(transform.position + randomness, damage.ToString(), Color.red);
        if (currentHealth <= 0)
        {
            Die();
        }
        
    }

    public void SetSpawner(EnemySpawner spawner)
    {
        this.spawner = spawner;
    }

    private void Die()
    {
        // Mo�esz doda� animacj� �mierci, usuni�cie obiektu itd.
        Debug.Log("Enemy died!");
        if (spawner != null)
        {
            spawner.CurrentEnemies.Remove(this.gameObject);
        }
        Destroy(gameObject); // Usuni�cie przeciwnika z gry
    }
}
