using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("=== SLIDERS ===")]
    public Slider HealthSlider;
    public Slider EaseHealthSlider;

    [Header("=== SETTINGS ===")]
    public float fadeOutDuration = 1f; // Czas zanikania paska zdrowia

    private float _maxHealth;
    private float _currentHealth;
    private float _lerpSpeed = 0.05f;
    private CanvasGroup _canvasGroup; // Obsługuje przezroczystość UI
    private bool _isFadingOut = false; // Flaga, czy pasek zanika

    void Awake()
    {
        // Znajdź CanvasGroup w hierarchii Canvas -> HealthBar
        _canvasGroup = GetComponentInChildren<CanvasGroup>();

        // Jeśli CanvasGroup nie istnieje, dodaj go do obiektu nadrzędnego HealthBar
        if (_canvasGroup == null)
        {
            Debug.Log("CanvasGroup not found. Adding one...");
            GameObject healthBarParent = transform.Find("Canvas/HealthBar").gameObject;
            _canvasGroup = healthBarParent.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        UpdateHealthUI();

        // Jeśli zdrowie wynosi 0, rozpocznij proces zanikania
        if (_currentHealth <= 0 && !_isFadingOut)
        {
            StartCoroutine(FadeOutHealthBar());
        }
    }

    public void Initialize(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;

        // Upewnij się, że pasek zdrowia jest widoczny na początku
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.gameObject.SetActive(true); // Włącz HealthBar, jeśli był wyłączony
        }
    }

    private void UpdateHealthUI()
    {
        float normalizedHealth = _currentHealth / _maxHealth * 100f;
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

        // Jeśli pasek zdrowia ma być przywrócony, resetujemy znikanie
        if (_currentHealth > 0 && _isFadingOut)
        {
            StopAllCoroutines();
            _isFadingOut = false;

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.gameObject.SetActive(true);
            }
        }
    }

    private System.Collections.IEnumerator FadeOutHealthBar()
    {
        _isFadingOut = true; // Zapobiega wielokrotnemu uruchomieniu korutyny
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = alpha; // Stopniowe zmniejszanie przezroczystości
            }

            yield return null;
        }

        // Po zakończeniu zanikania, całkowicie wyłącz pasek zdrowia
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false); // Wyłącz obiekt paska zdrowia
        }
    }
}
