using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private float Health;
    private float lerpTimer;
    [Header("Health Bar")]
    public float MaxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public Slider healthSlider;

    [Header("Damage Overlay")]
    public Image overlay;
    public float duration;
    public float fadeSpeed;

    private float durationTimer;
    // Start is called before the first frame update

    [Header("=== SLIDERS ===")]
    public Slider HealthSlider;
    public Slider EaseHealthSlider;
    private float _lerpSpeed = 0.05f;

    void Start()
    {
        Health = MaxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        UpdateHealthUI();
        if (overlay.color.a > 0)
        {
            if (Health < 30)
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
        if (HealthSlider.value != Health)
        {
            HealthSlider.value = Health;
        }

        if (HealthSlider.value != EaseHealthSlider.value)
        {
            EaseHealthSlider.value = Mathf.Lerp(EaseHealthSlider.value, Health, _lerpSpeed);
        }

    //    float fillF = frontHealthBar.fillAmount;
    //    float fillB = backHealthBar.fillAmount;
    //    float hFraction = Health / MaxHealth;
    //    if (fillB > hFraction)
    //    {
    //        frontHealthBar.fillAmount = hFraction;
    //        backHealthBar.color = Color.red;
    //        lerpTimer += Time.deltaTime;
    //        float percentComplete = lerpTimer / chipSpeed;
    //        percentComplete = percentComplete * percentComplete;
    //        backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
    //    }
    //    if (fillF < hFraction)
    //    {
    //        backHealthBar.color = Color.green;
    //        backHealthBar.fillAmount = hFraction;
    //        lerpTimer += Time.deltaTime;
    //        float percentComplete = lerpTimer/ chipSpeed;
    //        percentComplete = percentComplete * percentComplete;
    //        frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
    //    }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        lerpTimer = 0f;
        durationTimer = 0f;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
        if (Health <= 0)
        {
            GameOverManager.Instance.EndGame();
        }
    }

    public void RestoreHealth(float healAmount)
    {
        Health += healAmount;
        lerpTimer = 0f;
    }
}
