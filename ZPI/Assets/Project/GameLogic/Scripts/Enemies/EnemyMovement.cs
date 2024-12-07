using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    public float MinVelocity = 0.5f;
    public float CloseDistanceThreshold = 1.5f; // Minimalna odległość uznawana za "blisko celu"
    public float MaxStuckTime = 3f; // Maksymalny czas, po którym cel zostanie zresetowany
    public float OffsetAdjustmentSpeed = 5f; // Szybkość dostosowania offsetu
    public float OffsetTolerance = 0.01f; // Tolerancja różnicy w wysokości

    private NavMeshAgent _agent;
    private Animator _animator;
    [SerializeField] private LookAt LookAt;
    [SerializeField] private bool DebugMode = false;

    private Vector2 _velocity;
    private Vector2 _smoothDeltaPosition;
    private float _stuckTimer = 0f; // Licznik czasu, przez który agent jest "zablokowany"

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _animator.applyRootMotion = true;
        _agent.updatePosition = false;
        _agent.updateRotation = true;
    }

    private void Update()
    {
        SynchronizeAnimatorAndAgent();
        CheckForStuck();

        if (DebugMode)
        {
            if (Input.GetMouseButtonDown(1)) // Obsługa prawego przycisku myszy
            {
                MoveToMouseClickPosition();
            }
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = _animator.rootPosition;
        rootPosition.y = _agent.nextPosition.y;
        transform.position = rootPosition;
        _agent.nextPosition = rootPosition;
    }

    private void SynchronizeAnimatorAndAgent()
    {
        Vector3 worldDeltaPosition = _agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        _smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

        _velocity = _smoothDeltaPosition / Time.deltaTime;
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _velocity = Vector2.Lerp(Vector2.zero, _velocity, _agent.remainingDistance);
        }

        bool shouldMove = _velocity.magnitude > MinVelocity && _agent.remainingDistance > _agent.stoppingDistance;

        _animator.SetBool("move", shouldMove);
        _animator.SetFloat("velx", _velocity.x);
        _animator.SetFloat("vely", _velocity.y);

        LookAt.lookAtTargetPosition = _agent.steeringTarget + transform.forward;
    }

    private void MoveToMouseClickPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
            {
                _agent.SetDestination(navHit.position);
                _agent.isStopped = false;
                _stuckTimer = 0f; // Reset licznika "zablokowania"
            }
        }
    }

    public void StopMoving()
    {
        _agent.isStopped = true;
    }

    private void CheckForStuck()
    {
        if (_agent.remainingDistance <= CloseDistanceThreshold && _agent.velocity.magnitude < MinVelocity)
        {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= MaxStuckTime)
            {
                Debug.Log("Agent zablokowany. Reset celu.");
                _agent.SetDestination(transform.position);
                _stuckTimer = 0f;
            }
        }
        else
        {
            _stuckTimer = 0f;
        }
    }
}
