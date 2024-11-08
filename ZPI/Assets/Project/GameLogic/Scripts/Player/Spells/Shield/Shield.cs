using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject shieldObject; // Referencja do sfery tarczy
    public string enemyTag = "Enemy"; // Tag przeciwnika
    public string bulletTag = "Bullet"; // Tag pocisk�w przeciwnika

    private void Start()
    {
        // Wy��cz tarcz� na pocz�tku
        shieldObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            shieldObject.SetActive(!shieldObject.activeSelf);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (shieldObject.activeSelf)
        {
            if (other.tag == enemyTag || other.tag == bulletTag)
            {
                //Destroy(other.gameObject);
            }
        }
    }
}
