using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject shieldObject;
    public float shieldDuration = 5f;

    private void Start()
    {
        // Wy��cz tarcz� na pocz�tku
        shieldObject.SetActive(false);
    }

    private void Update()
    {
    }

    public void activateShield()
    {
        shieldObject.SetActive(true);
        Invoke(nameof(DeactivateShield), shieldDuration);
    }

    private void DeactivateShield()
    {
        shieldObject.SetActive(false);
    }
}
