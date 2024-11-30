using UnityEngine.AI;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    public float MinVelocity = 0.5f;
    public float CloseDistanceThreshold = 1.5f; // Minimalna odległość uznawana za "blisko celu"
    public float MaxStuckTime = 3f; // Maksymalny czas, po którym cel zostanie zresetowany

    private NavMeshAgent Agent;
    private Animator Animator;
    [SerializeField]
    private LookAt LookAt;
    [SerializeField]
    private bool DebugMode = false;

    private Vector2 Velocity;
    private Vector2 SmoothDeltaPosition;

    private float stuckTimer = 0f; // Licznik czasu, przez który agent jest "zablokowany"

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        Animator.applyRootMotion = true;
        Agent.updatePosition = false;
        Agent.updateRotation = true;
    }

    private void Update()
    {
        SynchronizeAnimatorAndAgent();
        CheckForStuck();

        // Obsługa prawego przycisku myszy
        if (DebugMode)
        {
            if (Input.GetMouseButtonDown(1)) // 1 oznacza prawy przycisk myszy
            {
                MoveToMouseClickPosition();
            }
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = Animator.rootPosition;
        rootPosition.y = Agent.nextPosition.y;
        transform.position = rootPosition;
        Agent.nextPosition = rootPosition;
    }

    private void SynchronizeAnimatorAndAgent()
    {
        Vector3 worldDeltaPosition = Agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        SmoothDeltaPosition = Vector2.Lerp(SmoothDeltaPosition, deltaPosition, smooth);

        Velocity = SmoothDeltaPosition / Time.deltaTime;
        if (Agent.remainingDistance <= Agent.stoppingDistance)
        {
            Velocity = Vector2.Lerp(Vector2.zero, Velocity, Agent.remainingDistance);
        }

        bool shouldMove = Velocity.magnitude > MinVelocity && Agent.remainingDistance > Agent.stoppingDistance;

        Animator.SetBool("move", shouldMove);
        Animator.SetFloat("velx", Velocity.x);
        Animator.SetFloat("vely", Velocity.y);

        LookAt.lookAtTargetPosition = Agent.steeringTarget + transform.forward;
    }

    private void MoveToMouseClickPosition()
    {
        // Rzutowanie promienia z pozycji myszy
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Sprawdzenie, czy kliknięcie nastąpiło na NavMesh
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
            {
                // Ustawienie celu na pozycji kliknięcia
                Agent.SetDestination(navHit.position);
                Agent.isStopped = false;

                // Reset licznika "zablokowania"
                stuckTimer = 0f;
            }
        }
    }

    public void StopMoving()
    {
        Agent.isStopped = true;
    }

    private void CheckForStuck()
    {
        // Jeśli agent znajduje się blisko celu, ale nie dotarł do niego
        if (Agent.remainingDistance <= CloseDistanceThreshold && Agent.velocity.magnitude < MinVelocity)
        {
            stuckTimer += Time.deltaTime;

            // Jeśli agent jest "zablokowany" przez dłużej niż MaxStuckTime
            if (stuckTimer >= MaxStuckTime)
            {
                Debug.Log("Agent zablokowany. Reset celu.");
                Agent.SetDestination(transform.position); // Ustaw cel na bieżącą pozycję agenta
                stuckTimer = 0f; // Reset licznika
            }
        }
        else
        {
            // Jeśli agent porusza się prawidłowo, resetuj licznik
            stuckTimer = 0f;
        }
    }
}
