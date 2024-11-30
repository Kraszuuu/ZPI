using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    [Header("Health Bar")]
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;

    [Header("Damage Overlay")]
    public float duration;
    public float fadeSpeed;

    private float durationTimer;

    [Header("=== SLIDERS ===")]
    public Slider HealthSlider;
    public Slider EaseHealthSlider;
    private float _lerpSpeed = 0.05f;

    private PlayerMotor _playerMotor;

    private AudioManager _audioManager;

    void Start()
    {
        health = maxHealth;
        _audioManager = FindObjectOfType<AudioManager>();
        _playerMotor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        
    }

    public void UpdateHealthUI()
    {
        float normalizedHealth = (health / maxHealth) * 100f;
        if (HealthSlider.value != normalizedHealth)
        {
            HealthSlider.value = normalizedHealth;
        }

        if (EaseHealthSlider.value != normalizedHealth)
        {
            EaseHealthSlider.value = Mathf.Lerp(EaseHealthSlider.value, normalizedHealth, _lerpSpeed);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0f;
        if (CompareTag("Player"))
        {
            if (health <= 0)
            {
                GameOverManager.Instance.EndGame();
                _audioManager.PlayPlayerDeathSound();
            }
            else
            {
                _audioManager.PlayPlayerDamageSound();
            }
        }
        _playerMotor.ApplyDamageEffect();
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
        _audioManager.PlayHealSound();
    }

    public void RenewHealth()
    {
        health = maxHealth;
    }

    public void BuffHealth()
    {
        maxHealth += 100;
    }
}
