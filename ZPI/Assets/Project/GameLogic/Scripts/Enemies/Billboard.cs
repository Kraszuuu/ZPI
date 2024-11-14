using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform Cam;

    void Awake()
    {
        // Sprawdza, czy kamera jest przypisana. Je�li nie, przypisuje kamer� g��wn� ze sceny.
        if (Cam == null)
        {
            Cam = Camera.main?.transform;
        }

        if (Cam == null)
        {
            Debug.LogError("Main Camera not found in the scene!");
        }
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + Cam.forward);
    }
}
