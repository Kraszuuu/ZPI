using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionManager : MonoBehaviour
{
    // Statyczna referencja do jedynej instancji DetectionManager
    public static DetectionManager Instance { get; private set; }

    public bool PlayerDetected { get; private set; } = false;
    public Vector3 LastKnownPlayerPosition { get; private set; }

    [Header("Detection Settings")]
    public float detectionTimeout = 4f; // Czas w sekundach po którym gracz jest uznawany za zgubionego

    [SerializeField, ReadOnly]
    private float detectionTimer = 0f; // Licznik czasu od ostatniego zgłoszenia

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (PlayerDetected)
        {
            detectionTimer += Time.deltaTime;
        }

        // Jeśli licznik przekroczy czas limitu, uznaj gracza za zgubionego
        if (detectionTimer >= detectionTimeout)
        {
            Debug.Log("Gracz został zgubiony.");
            ResetDetection();
        }
    }

    // Metoda do zgłaszania wykrycia gracza przez dowolnego przeciwnika
    public void ReportPlayerDetected(Vector3 playerPosition)
    {
        if (!PlayerDetected)
        {
            Debug.Log("Gracz został wykryty.");
            PlayerDetected = true;
        }
        detectionTimer = 0f;
        LastKnownPlayerPosition = playerPosition;
    }

    // Metoda resetująca wykrycie gracza
    public void ResetDetection()
    {
        PlayerDetected = false;
        detectionTimer = 0f;
    }
}
