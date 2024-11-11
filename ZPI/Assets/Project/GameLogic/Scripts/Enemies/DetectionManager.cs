using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionManager : MonoBehaviour
{
    // Statyczna referencja do jedynej instancji DetectionManager
    public static DetectionManager Instance { get; private set; }

    public bool PlayerDetected { get; private set; } = false;
    public Vector3 LastKnownPlayerPosition { get; private set; }

    private void Awake()
    {
        // Sprawdzenie, czy instancja ju� istnieje
        if (Instance == null)
        {
            Instance = this; // Ustaw obecn� instancj�
            DontDestroyOnLoad(gameObject); // Zabezpiecza przed zniszczeniem obiektu przy zmianie scen
        }
        else
        {
            Destroy(gameObject); // Usu� now� instancj�, je�li ju� istnieje
        }
    }

    // Metoda do zg�aszania wykrycia gracza przez dowolnego przeciwnika
    public void ReportPlayerDetected(Vector3 playerPosition)
    {
        PlayerDetected = true;
        LastKnownPlayerPosition = playerPosition;
    }

    // Metoda resetuj�ca wykrycie gracza
    public void ResetDetection()
    {
        PlayerDetected = false;
    }
}
