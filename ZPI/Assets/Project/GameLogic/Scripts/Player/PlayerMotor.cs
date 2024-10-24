using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    public float WalkingSpeed = 5f;
    public float SprintingSpeed = 12f;
    public float MovementSharpnessOnGround = 15f;
    public float SprintHoldToLockThreshold = 2f;

    [Header("Movement in air")]
    public float MaxSpeedInAir = 12f;
    public float AccelerationSpeedInAir = 2f;

    [Header("Jumping")]
    public float GravityForce = -9.8f;
    public float JumpHeight = 3f;

    [Header("Dashing")]
    public float DashSpeed = 10f;
    public float DashDuration = 0.4f;
    public float DashCooldown = 1f;

    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private bool _isGrounded;
    private bool _isCrouching;
    private bool _lerpCrouch;
    private bool _isSprinting;
    private bool _isDashing = false;
    private bool _sprintLocked;
    private float _currentSpeed;
    private float _crouchTimer;
    private float _dashTimer;

    private float _sprintTimer = 0f;
    private bool _isMovingForward = false;
    private float _dashTime = 0f;
    private float _dashCooldownTime = 0f;
    private Vector3 _dashDirection;

    public bool isEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _currentSpeed = WalkingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = _controller.isGrounded;

        if (_lerpCrouch)
        {
            LerpCrouch();
        }

        if (_sprintLocked && !_isMovingForward)
        {
            _sprintLocked = false;
            _isSprinting = false;
            _currentSpeed = WalkingSpeed;
        }

        if (_isDashing)
        {
            PerformDash();
        }

        _dashCooldownTime -= Time.deltaTime;
    }

    // receive the inputs from InputManager.cs and apply them to the character _controller.
    public void ProcessMove(Vector2 input)
    {
        if (!_isDashing)
        {
            _isMovingForward = input.y > 0;

            Vector3 moveDirection = Vector3.zero;
            moveDirection.x = input.x;
            moveDirection.z = input.y;

            // Handle sprint locking after 2 seconds
            if (_isSprinting && _isMovingForward && !_sprintLocked)
            {
                _sprintTimer += Time.deltaTime;
                if (_sprintTimer >= SprintHoldToLockThreshold)
                {
                    _sprintLocked = true;
                    _currentSpeed = SprintingSpeed;
                    Debug.Log("Sprint locked!");
                }
            }
            else
            {
                _sprintTimer = 0f;
            }

            if (_isGrounded)
            {
                Vector3 targetVelocity = transform.TransformDirection(moveDirection) * _currentSpeed;

                _playerVelocity = Vector3.Lerp(_playerVelocity, targetVelocity, MovementSharpnessOnGround * Time.deltaTime);

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
        _isDashing = true;
        _dashTime = DashDuration;
        _dashCooldownTime = DashCooldown;
        _dashDirection = transform.TransformDirection(direction);
        _playerVelocity = direction * DashSpeed;
    }

    private void PerformDash()
    {
        _dashTime -= Time.deltaTime;

        if (_dashTime > 0.25)
        {
            _dashTimer += Time.deltaTime;
            float p = _dashTimer / 0.5f;
            p *= p;

            float targetHeight = 0.8f;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, p);

            Vector3 dashMovement = _dashDirection + Vector3.down;

            _controller.Move(dashMovement * DashSpeed * Time.deltaTime);
        }
        else if (_dashTime > 0)
        {
            _dashTimer -= Time.deltaTime;

            float p = (_dashTimer / 0.5f);
            p = 1 - p;
            p *= p;

            float targetHeight = 2f;
            _controller.height = Mathf.Lerp(_controller.height, targetHeight, p);

            Vector3 dashMovement = _dashDirection;

            _controller.Move(dashMovement * DashSpeed * Time.deltaTime);
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

    void HandleAirMovement(Vector3 moveDirection)
    {
        // Add air acceleration
        _playerVelocity += AccelerationSpeedInAir * Time.deltaTime * transform.TransformDirection(moveDirection);

        // Limit air speed horizontally
        float verticalVelocity = _playerVelocity.y;
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(_playerVelocity, Vector3.up);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir);

        _playerVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            _playerVelocity.y = Mathf.Sqrt(JumpHeight * -2.0f * GravityForce);
        }
    }

    public void Crouch()
    {
        _isCrouching = !_isCrouching;
        _crouchTimer = 0;
        _lerpCrouch = true;
    }

    // Slowly change camera height while in process of _isCrouching/standing up
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

    public void Sprint()
    {
        if (!_sprintLocked)
        {
            _isSprinting = !_isSprinting;
            _currentSpeed = _isSprinting ? SprintingSpeed : WalkingSpeed;
        }
    }
}

