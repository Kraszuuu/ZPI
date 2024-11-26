using UnityEngine;

public class Arrow : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Logika po trafieniu
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Zatrzymaj ruch
        }

        // Możesz tu dodać efekt wizualny lub obrażenia
        Debug.Log($"Arrow hit {collision.gameObject.name}");
    }
}
