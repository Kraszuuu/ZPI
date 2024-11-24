using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("=== SLIDERS ===")]
    public Slider HealthSlider;
    public Slider EaseHealthSlider;

    private float _maxHealth;
    private float _currentHealth;
    private float _lerpSpeed = 0.05f;

    void Update()
    {
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        UpdateHealthUI();
    }

    public void Initialize(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
    }

    private void UpdateHealthUI()
    {
        float normalizedHealth = (_currentHealth / _maxHealth) * 100f;
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
        _currentHealth -= damage;
    }

    public void RestoreHealth(float healAmount)
    {
        _currentHealth += healAmount;
    }
}
