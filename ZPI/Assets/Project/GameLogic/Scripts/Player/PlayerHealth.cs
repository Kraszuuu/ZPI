using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float _health;

    [Header("Health Bar")]
    public float MaxHealth = 100f;

    [Header("Mask and Health Bars")]
    public RectTransform Mask;               // Maska głównego paska zdrowia
    public RectTransform HealthBar;          // Główny pasek zdrowia (czerwony)

    public RectTransform EaseMask;           // Maska EaseHealthBar
    public RectTransform EaseHealthBar;      // EaseHealthBar (żółty)

    private float _originalMaskWidth;         // Początkowa szerokość maski
    private Vector2 _originalMaskPos;         // Początkowa pozycja maski

    private float _easeLerpSpeed = 5f;        // Szybkość interpolacji EaseHealthBar

    private AudioManager _audioManager;
    private PlayerMotor _playerMotor;

    [Header("Debugging")]
    public bool DebugTakeDamage = false;      // Debugowanie obrażeń
    public bool DebugRestoreHealth = false;   // Debugowanie przywracania zdrowia
    public float DebugDamageAmount = 10f;     // Ilość obrażeń do debugowania
    public float DebugHealAmount = 10f;       // Ilość zdrowia do debugowania

    void Start()
    {
        _health = MaxHealth;

        _audioManager = FindObjectOfType<AudioManager>();
        _playerMotor = GetComponent<PlayerMotor>();

        // Zapisujemy początkowe wartości maski
        _originalMaskWidth = Mask.rect.width;
        _originalMaskPos = Mask.anchoredPosition;
    }

    void Update()
    {
        _health = Mathf.Clamp(_health, 0, MaxHealth);
        UpdateHealthBars();

        HandleDebugging(); // Obsługa trybu debugowania
    }

    private void UpdateHealthBars()
    {
        // Oblicz procent zdrowia
        float healthPercentage = _health / MaxHealth;

        // Oblicz przesunięcie maski głównego paska
        float offset = (1 - healthPercentage) * _originalMaskWidth;

        // Przesuwamy maskę głównego paska w lewo
        Mask.anchoredPosition = new Vector2(_originalMaskPos.x - offset, _originalMaskPos.y);

        // Kompensujemy pozycję paska zdrowia w prawo
        HealthBar.anchoredPosition = new Vector2(offset, HealthBar.anchoredPosition.y);

        // Aktualizujemy EaseHealthBar (żółty pasek) z opóźnieniem
        UpdateEaseHealthBar(offset);
    }

    private void UpdateEaseHealthBar(float targetOffset)
    {
        // Interpolacja maski EaseHealthBar
        EaseMask.anchoredPosition = Vector2.Lerp(
            EaseMask.anchoredPosition,
            new Vector2(_originalMaskPos.x - targetOffset, _originalMaskPos.y),
            Time.deltaTime * _easeLerpSpeed
        );

        // Interpolacja obrazu EaseHealthBar
        EaseHealthBar.anchoredPosition = Vector2.Lerp(
            EaseHealthBar.anchoredPosition,
            new Vector2(targetOffset, EaseHealthBar.anchoredPosition.y),
            Time.deltaTime * _easeLerpSpeed
        );
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (CompareTag("Player"))
        {
            if (_health <= 0)
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
        _health += healAmount;
        _audioManager.PlayHealSound();
    }

    public void RenewHealth()
    {
        _health = MaxHealth;
    }

    public void BuffHealth()
    {
        MaxHealth += 100;
        _health += 100;
    }

    private void HandleDebugging()
    {
        if (DebugTakeDamage)
        {
            DebugTakeDamage = false; // Wyłącz opcję po jednokrotnym użyciu
            TakeDamage(DebugDamageAmount);
        }

        if (DebugRestoreHealth)
        {
            DebugRestoreHealth = false; // Wyłącz opcję po jednokrotnym użyciu
            RestoreHealth(DebugHealAmount);
        }
    }
}
