using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    public float MaxStamina = 100f;
    public float StaminaConsumptionRate = 25f;
    public float StaminaRegenRate = 10f;
    public float IsExhaustedValue = 30f;
    public float CurrentStamina;

    [Header("UI")]
    public RectTransform StaminaBarMask; // Maska przesuwana w lewo
    public RectTransform StaminaBarImage; // Obraz przesuwany w prawo
    public Color NormalColor = Color.yellow;
    public Color ExhaustedColor = Color.red;
    public float FlashDuration = 0.5f;

    private bool _isExhausted = false;
    private bool _isFlashing;
    private float _flashTimer;
    private float _initialMaskPositionX; // Początkowa pozycja maski
    private float _initialImagePositionX; // Początkowa pozycja obrazu
    private float _maskWidth;

    public bool IsExhausted => _isExhausted;

    private void Start()
    {
        CurrentStamina = MaxStamina;

        // Zapisz początkowe wartości pozycji maski i obrazu
        _initialMaskPositionX = StaminaBarMask.localPosition.x;
        _initialImagePositionX = StaminaBarImage.localPosition.x;

        // Oblicz szerokość maski (zakładamy, że jest prostokątna)
        _maskWidth = StaminaBarMask.rect.width;

        UpdateStaminaBar();
    }

    public void ConsumeStamina()
    {
        CurrentStamina -= StaminaConsumptionRate * Time.deltaTime;
        if (CurrentStamina <= 0)
        {
            CurrentStamina = 0;
            _isExhausted = true;
        }
        UpdateStaminaBar();
    }

    public void RegenerateStamina()
    {
        CurrentStamina += StaminaRegenRate * Time.deltaTime;
        if (CurrentStamina >= MaxStamina)
        {
            CurrentStamina = MaxStamina;
        }

        if (CurrentStamina >= IsExhaustedValue)
        {
            _isExhausted = false;
        }

        UpdateStaminaBar();
    }

    private void UpdateStaminaBar()
    {
        // Oblicz procent staminy
        float staminaPercent = CurrentStamina / MaxStamina;

        // Przesuń maskę w lewo proporcjonalnie do staminy
        float newMaskPositionX = _initialMaskPositionX - (1 - staminaPercent) * _maskWidth;
        StaminaBarMask.localPosition = new Vector3(newMaskPositionX, StaminaBarMask.localPosition.y, StaminaBarMask.localPosition.z);

        // Przesuń obraz w prawo proporcjonalnie do staminy
        float newImagePositionX = _initialImagePositionX + (1 - staminaPercent) * _maskWidth;
        StaminaBarImage.localPosition = new Vector3(newImagePositionX, StaminaBarImage.localPosition.y, StaminaBarImage.localPosition.z);

        // Obsługa migania
        HandleFlashing();
    }

    private void HandleFlashing()
    {
        if (_isExhausted && !_isFlashing)
        {
            StartFlashing();
        }
        else if (!_isExhausted && _isFlashing)
        {
            StopFlashing();
        }

        if (_isFlashing)
        {
            FlashStaminaBar();
        }
    }

    private void FlashStaminaBar()
    {
        _flashTimer += Time.deltaTime;
        if (StaminaBarImage != null)
        {
            float lerp = Mathf.PingPong(_flashTimer, FlashDuration) / FlashDuration;
            StaminaBarImage.GetComponent<Image>().color = Color.Lerp(NormalColor, ExhaustedColor, lerp);
        }
    }

    private void StartFlashing()
    {
        _isFlashing = true;
        _flashTimer = 0f;
    }

    private void StopFlashing()
    {
        _isFlashing = false;
        if (StaminaBarImage != null)
        {
            StaminaBarImage.GetComponent<Image>().color = NormalColor;
        }
    }
}
