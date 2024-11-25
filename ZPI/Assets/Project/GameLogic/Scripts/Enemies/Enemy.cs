using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent Agent { get => _agent; }
    public GameObject Player { get => _player; }
    public StateMachine StateMachine { get => _stateMachine; }

    [Header("Enemy class")]
    public EnemyType enemyType;

    [Header("Health")]
    public int MaxHealth = 100;

    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight;

    [Header("Weapon Values")]
    public Transform gunBarrel;
    public GameObject bulletPrefab;
    [Range(0.1f, 10)]
    public float fireRate;

    private StateMachine _stateMachine;
    private NavMeshAgent _agent;
    private GameObject _player;
    private Ragdoll _ragdoll;
    private Animator _animator;
    private EnemySpawner _spawner;
    private DamagePopupGenerator _damagePopupGenerator;
    private EnemyHealthBar _healthBar;

    [Header("Debug Info")]
    [SerializeField, ReadOnly]
    private string _currentState;
    [SerializeField, ReadOnly]
    private int _currentHealth;

    void Start()
    {
        _stateMachine = GetComponent<StateMachine>();
        _agent = GetComponent<NavMeshAgent>();
        _ragdoll = GetComponent<Ragdoll>();
        _animator = GetComponent<Animator>();
        _healthBar = GetComponent<EnemyHealthBar>();
        _player = GameObject.FindGameObjectWithTag("Player");

        _damagePopupGenerator = GetComponentInChildren<DamagePopupGenerator>();

        _currentHealth = MaxHealth;
        _healthBar.Initialize(MaxHealth);
        _stateMachine.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        _currentState = _stateMachine.activeState.ToString();
        CanSeePlayer();
    }

    public bool CanSeePlayer()
    {
        if (_player == null) return false;

        if (Vector3.Distance(transform.position, _player.transform.position) >= sightDistance) return false;

        Vector3 targetDirection = _player.transform.position - transform.position - (Vector3.up * eyeHeight);
        float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
        if (angleToPlayer < -fieldOfView || angleToPlayer > fieldOfView) return false;

        Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
        RaycastHit hitInfo = new RaycastHit();

        int layerToIgnore = LayerMask.NameToLayer("Shield");
        int layerMask = ~(1 << layerToIgnore);

        if (!Physics.Raycast(ray, out hitInfo, sightDistance, layerMask) || hitInfo.transform.gameObject != _player) return false;

        Debug.DrawRay(ray.origin, ray.direction * sightDistance, Color.yellow);

        return true;
    }

    public void TakeDamage(int damage, Vector3 hitForceVector)
    {
        _currentHealth -= damage;
        _healthBar.TakeDamage(damage);
        _animator.SetTrigger("gotHit");

        Vector3 randomness = new Vector3(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
        _damagePopupGenerator.CreatePopup(transform.position + randomness, damage.ToString(), Color.yellow);

        if (_currentHealth <= 0)
        {
            Die(hitForceVector);
        }

        if (!DetectionManager.Instance.PlayerDetected)
        {
            _animator.SetTrigger("scream");
            _stateMachine.SetAnimatorBool("chase", true);
            _stateMachine.ChangeState(new AttackState());
        }
        DetectionManager.Instance.ReportPlayerDetected(_player.transform.position);
    }

    public void SetSpawner(EnemySpawner spawner)
    {
        this._spawner = spawner;
    }

    private void Die(Vector3 hitForceVector)
    {
        if (_spawner != null)
        {
            _spawner.CurrentEnemies.Remove(this.gameObject);
        }
        _agent.isStopped = true;
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _ragdoll.EnableRagdoll();
        Debug.Log(hitForceVector);
        _ragdoll.ApplyForce(hitForceVector);
        StartCoroutine(FadeOutAndDestroy());
    }

    // Korutyna obsługująca zanikanie
    private IEnumerator FadeOutAndDestroy()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float fadeDuration = 3f; // Czas zanikania
        float timer = 0f;

        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;

        // Zapewnij, że wszystkie materiały są w trybie transparent
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                material.SetFloat("_Mode", 2); // Tryb transparentny
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;

                if (material.HasProperty("_Color"))
                {
                    Color color = material.color;
                    color.a = 1f;
                    material.color = color;
                }
            }
        }

        // Stopniowe zmniejszanie alpha
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (material.HasProperty("_Color"))
                    {
                        Color color = material.color;
                        color.a = alpha;
                        material.color = color;
                    }
                }
            }

            yield return null;
        }

        // Zniszcz obiekt po zakończeniu zanikania
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Color oldColor = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * eyeHeight);
        Gizmos.color = oldColor;
    }
}
