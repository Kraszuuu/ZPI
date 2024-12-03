using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    public float destroyAfterSeconds = 5f; // Czas życia strzały po wbiciu
    public bool isHeld = true;
    private Collider arrowCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Upewnij się, że grawitacja działa
        rb.useGravity = true;
        arrowCollider = GetComponent<Collider>();
    }

    public void Initialize(GameObject shooter)
    {
        // Pobierz wszystkie Collidery strzelającego
        Collider[] shooterColliders = shooter.GetComponentsInChildren<Collider>();

        // Ignoruj kolizję między strzałą a każdym colliderem strzelającego
        foreach (Collider shooterCollider in shooterColliders)
        {
            if (arrowCollider != null && shooterCollider != null)
            {
                Physics.IgnoreCollision(arrowCollider, shooterCollider);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isHeld) return;

        // Zatrzymaj ruch strzały
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        arrowCollider.enabled = false;

        // Obsługa różnych typów obiektów
        if (collision.gameObject.CompareTag("Player"))
        {
            // Zadaj obrażenia graczowi i usuń strzałę
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Zadaj obrażenia wrogowi i usuń strzałę
            collision.gameObject.GetComponent<Enemy>().TakeDamage(10, transform.forward);
            Destroy(gameObject);
        }
        else
        {
            // Wbij strzałę w obiekt terenowy i usuń po czasie
            transform.parent = collision.transform; // Ustaw obiekt jako dziecko trafionego
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            AudioManager.instance.PlayArrowHitSound(audioSource);
            Destroy(gameObject, destroyAfterSeconds);
        }
    }

    void FixedUpdate()
    {
        // Sprawdź, czy strzała się porusza i nie jest "zamrożona"
        if (rb != null && !rb.isKinematic && rb.velocity.magnitude > 0.1f && !isHeld)
        {
            // Obrót w kierunku ruchu
            transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
            rb.AddTorque(Vector3.right * 0.5f, ForceMode.Acceleration); // Minimalny moment obrotowy
        }
    }
}
