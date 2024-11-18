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
    }

    public void activateShield()
    {
        shieldObject.SetActive(true);
        AudioManager.instance.PlayShieldSound();
        Invoke(nameof(DeactivateShield), SpellManager.Instance.GetSpellData("Shield"));
    }

    private void DeactivateShield()
    {
        shieldObject.SetActive(false);
    }
}
