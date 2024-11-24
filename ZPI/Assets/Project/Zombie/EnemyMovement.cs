using UnityEngine.AI;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    public float MinVelocity = 0.5f;
    private NavMeshAgent Agent;
    private Animator Animator;
    [SerializeField]
    private LookAt LookAt;
    private bool DebugMode = false;

    private Vector2 Velocity;
    private Vector2 SmoothDeltaPosition;

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
            }
        }
    }

    public void StopMoving()
    {
        Agent.isStopped = true;
    }
}
