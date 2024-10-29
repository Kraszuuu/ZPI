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
    public Image overlay;
    public float duration;
    public float fadeSpeed;

    private float durationTimer;

    [Header("=== SLIDERS ===")]
    public Slider HealthSlider;
    public Slider EaseHealthSlider;
    private float _lerpSpeed = 0.05f;

    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        if (overlay.color.a > 0)
        {
            if (health < 30)
            {
                return;
            }
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
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
        //float fillF = frontHealthBar.fillAmount;
        //float fillB = backHealthBar.fillAmount;
        //float hFraction = health / maxHealth;
        //if (fillB > hFraction)
        //{
        //    frontHealthBar.fillAmount = hFraction;
        //    backHealthBar.color = Color.red;
        //    lerpTimer += Time.deltaTime;
        //    float percentComplete = lerpTimer / chipSpeed;
        //    percentComplete = percentComplete * percentComplete;
        //    backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        //}
        //if (fillF < hFraction)
        //{
        //    backHealthBar.color = Color.green;
        //    backHealthBar.fillAmount = hFraction;
        //    lerpTimer += Time.deltaTime;
        //    float percentComplete = lerpTimer/ chipSpeed;
        //    percentComplete = percentComplete * percentComplete;
        //    frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        //}
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0f;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
        if (health <= 0)
        {
            GameOverManager.Instance.EndGame();
        }
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }
}
