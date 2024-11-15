using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    [Header("Health Bar")]
    public float maxHealth = 100f;
    public float chipSpeed = 2f;

    [Header("Damage Overlay")]
    public float duration;
    public float fadeSpeed;

    private float durationTimer;

    [Header("=== SLIDERS ===")]
    public Slider HealthSlider;
    public Slider EaseHealthSlider;
    private float _lerpSpeed = 0.05f;

    private AudioManager _audioManager;

    void Start()
    {
        health = maxHealth;
        _audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        
    }

    public void UpdateHealthUI()
    {
        if (HealthSlider.value != health)
        {
            HealthSlider.value = health;
        }

        if (HealthSlider.value != EaseHealthSlider.value)
        {
            EaseHealthSlider.value = Mathf.Lerp(EaseHealthSlider.value, health, _lerpSpeed);
        }  
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0f;
        if (health <= 0)
        {
            if (CompareTag("Player"))
            {
                GameOverManager.Instance.EndGame();
            }
            _audioManager.TakeDamageSound();
        }
        else
        {
            _audioManager.PlayerDieSound();
        }
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
        _audioManager.PlayerHealSound();
    }
}
