using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField]
    private GameObject ragdollObject; // Referencja do modelu z ragdollem
    private Animator animator;

    private Collider mainCollider;
    private Rigidbody mainRigidbody;
    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;

    private void Awake()
    {
        mainCollider = GetComponent<Collider>();
        mainRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Pobranie wszystkich colliderów i rigidbody z obiektu ragdolla
        ragdollColliders = ragdollObject.GetComponentsInChildren<Collider>();
        ragdollRigidbodies = ragdollObject.GetComponentsInChildren<Rigidbody>();

        // Wyłączenie ragdolla na początku
        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        // Wyłączenie głównego collidera i rigidbody
        if (mainCollider != null) mainCollider.enabled = false;
        if (mainRigidbody != null) mainRigidbody.isKinematic = true;

        // Włączenie colliderów i rigidbody w ragdollu
        foreach (var col in ragdollColliders)
        {
            col.enabled = true;
        }

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        animator.enabled = false;
    }

    public void DisableRagdoll()
    {
        // Włączenie głównego collidera i rigidbody
        if (mainCollider != null) mainCollider.enabled = true;
        if (mainRigidbody != null) mainRigidbody.isKinematic = false;

        // Wyłączenie colliderów i rigidbody w ragdollu
        foreach (var col in ragdollColliders)
        {
            col.enabled = false;
        }

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }

        animator.enabled = true;
    }
}
