using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float MinSpeed = 0.5f;
    public float StopThreshold = 1.5f;
    public float StuckDuration = 3f;

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebug = false;

    private NavMeshAgent _navMeshAgent;
    private LookAt _lookAt;
    private Animator _animator;
    private Vector2 _smoothDeltaPos;
    private Vector2 _velocity;
    private float _stuckTime = 0f;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _lookAt = GetComponent<LookAt>();
        InitializeAgent();
    }

    private void Update()
    {
        UpdateAnimator();
        HandleStuckDetection();

        if (enableDebug && Input.GetMouseButtonDown(1))
        {
            SetDestinationToMousePosition();
        }
    }

    private void OnAnimatorMove()
    {
        SyncRootMotionWithAgent();
    }

    private void InitializeAgent()
    {
        _animator.applyRootMotion = true;
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;
    }

    private void SyncRootMotionWithAgent()
    {
        Vector3 newPosition = _animator.rootPosition;
        newPosition.y = _navMeshAgent.nextPosition.y;
        transform.position = newPosition;
        _navMeshAgent.nextPosition = newPosition;
    }

    private void UpdateAnimator()
    {
        Vector3 worldOffset = _navMeshAgent.nextPosition - transform.position;
        worldOffset.y = 0;

        float localX = Vector3.Dot(transform.right, worldOffset);
        float localY = Vector3.Dot(transform.forward, worldOffset);
        Vector2 localDeltaPos = new Vector2(localX, localY);

        float smoothFactor = Mathf.Clamp01(Time.deltaTime / 0.1f);
        _smoothDeltaPos = Vector2.Lerp(_smoothDeltaPos, localDeltaPos, smoothFactor);

        _velocity = _smoothDeltaPos / Time.deltaTime;

        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _velocity = Vector2.Lerp(Vector2.zero, _velocity, _navMeshAgent.remainingDistance);
        }

        bool isMoving = _velocity.magnitude > MinSpeed && _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance;

        _animator.SetBool("move", isMoving);
        _animator.SetFloat("velx", _velocity.x);
        _animator.SetFloat("vely", _velocity.y);

        if (_lookAt != null)
        {
            _lookAt.lookAtTargetPosition = _navMeshAgent.steeringTarget + transform.forward;
        }
    }

    private void SetDestinationToMousePosition()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
            {
                _navMeshAgent.SetDestination(navHit.position);
                _navMeshAgent.isStopped = false;
                _stuckTime = 0f;
            }
        }
    }

    private void HandleStuckDetection()
    {
        bool isCloseToTarget = _navMeshAgent.remainingDistance <= StopThreshold;
        bool isVelocityLow = _navMeshAgent.velocity.magnitude < MinSpeed;

        if (isCloseToTarget && isVelocityLow)
        {
            _stuckTime += Time.deltaTime;
            if (_stuckTime >= StuckDuration)
            {
                Debug.Log("Agent appears stuck. Resetting destination.");
                _navMeshAgent.SetDestination(transform.position);
                _stuckTime = 0f;
            }
        }
        else
        {
            _stuckTime = 0f;
        }
    }
}
