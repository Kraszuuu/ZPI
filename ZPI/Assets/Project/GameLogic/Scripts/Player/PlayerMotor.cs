using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    public float WalkingSpeed = 4f;
    public float SprintingSpeed = 8f;
    public float CrouchingSpeed = 2f;
    public float MovementSharpnessOnGround = 15f;
    public float SprintHoldToLockThreshold = 2f;

    [Header("Movement in air")]
    public float AccelerationSpeedInAir = 2f;

    [Header("Jumping")]
    public float GravityForce = -9.8f;
    public float JumpHeight = 3f;

    [Header("Dashing")]
    public float DashSpeed = 10f;
    public float DashDuration = 0.4f;
    public float DashCooldown = 1f;

    [Header("Stamina")]
    public float MaxStamina = 100f;
    public float StaminaConsumptionRate = 25f;
    public float StaminaRegenRate = 10f;
    public float IsExhaustedValue = 30f;
    public float CurrentStamina;
    public Slider StaminaSlider;
    private bool _isExhausted = false;
    private Image _sliderFill;
    private bool _isFlashing;
    private float _flashTimer;
    public float FlashDuration = 0.5f;
    public Color ExhaustedColor = Color.red;
    public Color NormalColor = Color.yellow;

    [Header("Slow Movement")]
    public float SlowDuration = 5f;
    public float SlowMultiplier = 0.8f;
    public GameObject DamageEffectImage;
    private int _activeDamageEffects = 0;

    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private bool _isGrounded;
    private bool _isCrouching;
    private bool _lerpCrouch;
    private bool _isSprinting;
    private bool _isDashing = false;
    private float _crouchTimer;
    private float _dashTimer;
    private float _currentSpeed;

    private float _dashTime = 0f;
    private float _dashCooldownTime = 0f;
    private Vector3 _dashDirection;

    public bool isEnabled = true;

    //Sound related
    private AudioManager _audioManager;
    private float _stepSoundTimer = 0f;
    private float _stepSoundInterval = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _currentSpeed = WalkingSpeed;
        _audioManager = FindObjectOfType<AudioManager>();
        CurrentStamina = MaxStamina;
        _sliderFill = StaminaSlider.fillRect.GetComponent<Image>();
    }

    void Update()
    {
        CheckGroundStatus();
        HandleStamina();
        HandleFlashing();
        HandleCrouch();
        HandleDash();
        UpdateDamageEffects();
    }

    public void ProcessMove(Vector2 input)
    {
        if (!_isDashing)
        {

            Vector3 moveDirection = Vector3.zero;
            moveDirection.x = input.x;
            moveDirection.z = input.y;

            if (_isGrounded)
            {
                if (_controller.velocity.magnitude > 0.01f)
                {
                    if (_isSprinting) _stepSoundInterval = 0.25f;
                    else if (_isCrouching) _stepSoundInterval = 1f;
                    else _stepSoundInterval = 0.5f;
                    PlayStepSound();
                }

                Vector3 targetVelocity = transform.TransformDirection(moveDirection) * _currentSpeed;

                if (Vector3.Distance(_playerVelocity, targetVelocity) < 0.01f)
                {
                    // Ustawiamy bezpośrednio, gdy różnica jest bardzo mała
                    _playerVelocity = targetVelocity;
                }
                else
                {
                    // W przeciwnym razie interpolujemy jak zwykle
                    _playerVelocity = Vector3.Lerp(_playerVelocity, targetVelocity, MovementSharpnessOnGround * Time.deltaTime);
                }

                if (_playerVelocity.y < 0)
                {
                    _playerVelocity.y = -2f;
                }
            }
            else
            {
                // Handle air movement
                HandleAirMovement(moveDirection);
            }

            // Apply gravity
            _playerVelocity.y += GravityForce * Time.deltaTime;

            // Move the character
            _controller.Move(_playerVelocity * Time.deltaTime);
        }
    }

    public void StartDash(Vector3 direction)
    {
        if (_dashCooldownTime <= 0f && !_isDashing && _isGrounded)
        {
            _audioManager.PlayDashSound();
            _isDashing = true;
            _dashTime = DashDuration;
            _dashCooldownTime = DashCooldown;
            _dashDirection = transform.TransformDirection(direction);
            _playerVelocity = direction * DashSpeed;

            _isCrouching = false;
            _isSprinting = false;
            _currentSpeed = WalkingSpeed;
        }
    }

    public void HandleAirMovement(Vector3 moveDirection)
    {
        // Add air acceleration
        _playerVelocity += AccelerationSpeedInAir * Time.deltaTime * transform.TransformDirection(moveDirection);

        // Limit air speed horizontally
        float verticalVelocity = _playerVelocity.y;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_playerVelocity, Vector3.up);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, _currentSpeed);

        _playerVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            AudioManager.instance.PlayJumpSound();
            _playerVelocity.y = Mathf.Sqrt(JumpHeight * -2.0f * GravityForce);
        }
    }

    public void Crouch()
    {
        if (_isSprinting) _isSprinting = false;
        _isCrouching = !_isCrouching;
        _crouchTimer = 0;
        _lerpCrouch = true;
        _currentSpeed = _isCrouching ? CrouchingSpeed : WalkingSpeed;
    }

    public void ApplyDamageEffect()
    {
        StartCoroutine(ReduceSpeedTemporarily());
    }

    public void StartSprint()
    {
        if (!_isCrouching && _isGrounded && !_isExhausted)
        {
            _isSprinting = true;
            _currentSpeed = SprintingSpeed;
        }
    }

    public void StopSprint()
    {
        _isSprinting = false;
        _currentSpeed = WalkingSpeed;
    }

    private void CheckGroundStatus()
    {
        _isGrounded = _controller.isGrounded;
    }

    private void HandleStamina()
    {
        if (_isSprinting)
        {
            ConsumeStamina();
        }
        else
        {
            RegenerateStamina();
        }

        UpdateStaminaSlider();
    }

    private void ConsumeStamina()
    {
        CurrentStamina -= StaminaConsumptionRate * Time.deltaTime;
        if (CurrentStamina <= 0)
        {
            CurrentStamina = 0;
            _isExhausted = true;
            _isSprinting = false;
            _currentSpeed = WalkingSpeed;
        }
    }

    private void RegenerateStamina()
    {
        CurrentStamina += StaminaRegenRate * Time.deltaTime;
        if (CurrentStamina >= MaxStamina)
        {
            CurrentStamina = MaxStamina;
        }
        else if (CurrentStamina >= 30)
        {
            _isExhausted = false;
        }
    }

    private void UpdateStaminaSlider()
    {
        if (StaminaSlider != null)
        {
            StaminaSlider.value = CurrentStamina;
        }
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

    private void HandleCrouch()
    {
        if (_lerpCrouch)
        {
            LerpCrouch();
        }
    }

    private void HandleDash()
    {
        if (_isDashing)
        {
            PerformDash();
        }

        if (_dashCooldownTime > 0)
        {
            _dashCooldownTime -= Time.deltaTime;
        }
    }

    private void UpdateDamageEffects()
    {
        DamageEffectImage.SetActive(_activeDamageEffects > 0);
    }

    private void FlashStaminaBar()
    {
        _flashTimer += Time.deltaTime;
        if (_sliderFill != null)
        {
            float lerp = Mathf.PingPong(_flashTimer, FlashDuration) / FlashDuration;
            _sliderFill.color = Color.Lerp(NormalColor, ExhaustedColor, lerp);
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
        if (_sliderFill != null)
        {
            _sliderFill.color = NormalColor;
        }
    }

    private void PlayStepSound()
    {
        if (_stepSoundTimer <= 0f)
        {
            _audioManager.PlayWalkSound();
            _stepSoundTimer = _stepSoundInterval;
        }

        if (_stepSoundTimer > 0f)
        {
            _stepSoundTimer -= Time.deltaTime;
        }
    }

    private void PerformDash()
    {
        _dashTime -= Time.deltaTime;

        if (_dashTime > 0.2)
        {
            _dashTimer += Time.deltaTime;
            float p = _dashTimer / 0.4f;
            p *= p;

            float targetHeight = 0.8f;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, p);

            Vector3 dashMovement = _dashDirection + Vector3.down;

            _controller.Move(dashMovement * DashSpeed * Time.deltaTime);
        }
        else if (_dashTime > 0)
        {
            _dashTimer -= Time.deltaTime;

            float p = _dashTimer / 0.4f;
            p = 1 - p;
            p *= p;

            float targetHeight = 2f;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, p);

            Vector3 dashMovement = _dashDirection;

            _controller.Move(DashSpeed * Time.deltaTime * dashMovement);
        }
        else if (_dashTime <= 0)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        _isDashing = false;
        _dashTimer = 0f;
        _playerVelocity = Vector3.zero;
    }


    private IEnumerator ReduceSpeedTemporarily()
    {
        _activeDamageEffects += 1;
        Debug.Log(_activeDamageEffects);
        WalkingSpeed *= SlowMultiplier;
        SprintingSpeed *= SlowMultiplier;

        _currentSpeed = _isSprinting ? SprintingSpeed : WalkingSpeed;

        yield return new WaitForSeconds(SlowDuration);

        _activeDamageEffects -= 1;
        WalkingSpeed /= SlowMultiplier;
        SprintingSpeed /= SlowMultiplier;

        _currentSpeed = _isSprinting ? SprintingSpeed : WalkingSpeed;
    }

    private void LerpCrouch()
    {
        _crouchTimer += Time.deltaTime;
        float p = _crouchTimer / 1;
        p *= p;
        _controller.height = _isCrouching ? Mathf.Lerp(_controller.height, 1, p) : Mathf.Lerp(_controller.height, 2, p);

        if (p > 1)
        {
            _lerpCrouch = false;
            _crouchTimer = 0f;
        }
    }
}

