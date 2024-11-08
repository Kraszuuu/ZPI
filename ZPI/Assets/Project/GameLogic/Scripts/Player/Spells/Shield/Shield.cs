using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject shieldObject;

    private void Start()
    {
        // Wy��cz tarcz� na pocz�tku
        shieldObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(shieldObject);
            shieldObject.SetActive(!shieldObject.activeSelf);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (shieldObject.activeSelf)
        {
        }
    }
}
