using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeal : Interactable
{
    private Animator animator;
    private bool isHealing = false;
    private Health playerHealth;
    public int HealingAmount = 20;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerHealth = FindObjectOfType<Health>();
    }

    protected override void Interact()
    {
        if (!isHealing)
        {
            animator.SetBool("isHealing", true);
            isHealing = true;
            playerHealth.RestoreHealth(20);
        }
    }
}
