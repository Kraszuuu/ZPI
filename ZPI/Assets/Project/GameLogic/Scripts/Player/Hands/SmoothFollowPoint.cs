using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothFollowPoint : MonoBehaviour
{
    [Header("Unarmed Settings")]
    public float UnarmedSmoothTime = 0.15f;         // Czas amortyzacji pozycji
    public float RotationSmoothTime = 0.15f; // Czas amortyzacji rotacji
    public float HorizontalRotationSmoothTime = 0.025f; // Czas amortyzacji poziomego obrotu (lewo-prawo)
    public Vector3 UnarmedOffset = new(0.34f, -0.16f, 0.44f);           // Offset w lokalnych współrzędnych względem kamery
    public Quaternion UnarmedRotationOffset = new(0.4f, 0f, 0f, 0f);     // Offset rotacji między punktem a kamerą

    [Header("Wand Settings")]
    public float WandSmoothTime = 0.15f;
    public Vector3 WandOffset;               // Offset przy trzymaniu różdżki
    public Quaternion WandRotationOffset = new(0.4f, 0f, 0f, 0f);
    public GameObject WandObject;            // Referencja do obiektu różdżki

    [Header("Spell Casting Settings")]
    public float CastingSmoothTime = 0.02f; // Czas amortyzacji ruchu punktu
    public float DistanceFromCamera = 0.53f; // Odległość punktu od kamery
    public Vector3 CastingOffset;         // Offset w lokalnych współrzędnych względem kamery
    public float FollowStrength = 0.5f;            // Siła ruchu za myszą
    public float TransitionSpeed = 10f;

    [Header("Walking Noise Settings")]
    [Range(0, 0.1f)] public float NoiseAmplitude = 0.02f;
    [Range(0, 20)] public float NoiseFrequency = 15.0f;
    public float SpeedThreshold = 3.0f;
    public CharacterController CharacterController;

    [Header("Jumping Noise Settings")]
    [Range(0, 0.1f)] public float JumpNoiseAmplitude = 0.03f;

    [Header("Wand Swing Settings")]
    public float SwingSmoothTime = 0.075f;        // Szybkość machnięcia w lewo
    public float SwingHoldTime = 3f;     // Czas zatrzymania w miejscu po machnięciu
    public Vector3 LeftSwingOffset = new(-0.09f, -0.21f, 0.41f);  // Offset machnięcia w lewo
    public Quaternion LeftSwingRotationOffset = new(53.7f, 41.9f, 267f, 0f);
    public Vector3 RightSwingOffset = new(0.37f, -0.23f, 0.41f);  // Offset machnięcia w prawo
    public Quaternion RightSwingRotationOffset = new(85f, 129.4f, 277.1f, 0f);
    public float RotationSpeed = 5.0f;

    [Header("Debug")]
    [SerializeField] private Vector3 _currentTargetOffset;
    [SerializeField] private Quaternion _currentRotationOffset;

    private bool _isSwinging = false;      // Czy ręka aktualnie macha
    private bool _isLeftSwing = false;      // Czy ręka aktualnie machnela w lewo
    private float _swingTimer = 0f;        // Timer do zatrzymania w miejscu
    private Vector3 _velocity = Vector3.zero;  // Prędkość punktu, wymagana przez SmoothDamp
    private Camera _mainCamera;
    private Transform _cameraTransform;

    public Animator Animator;

    private bool _isToggleWandLocked = false;
    private float _interpolationDelayTimer = 0f;

    private float _currentSmoothTime;
    private float _currentYaw;              // Bieżąca wartość obrotu w poziomie (Y)
    private float _currentPitch;            // Bieżąca wartość obrotu w pionie (X)
    private Quaternion _targetRotation;
    private Quaternion _currentRotation;


    void Start()
    {
        _mainCamera = Camera.main;
        _cameraTransform = _mainCamera.transform;

        _currentYaw = _cameraTransform.eulerAngles.y;
        _currentPitch = _cameraTransform.eulerAngles.x;

        // Na początku różdżka jest widoczna
        GameState.Instance.IsWandEquipped = true;
        if (WandObject != null)
            WandObject.SetActive(true);
    }

    void Update()
    {
        if (!_isSwinging)
        {
            UpdateOffsetsAndSmoothTime();
        }

        if (GameState.Instance.IsSpellCasting)
        {
            HandleSpellCasting();
            return;
        }

        Vector3 noise = CalculateNoiseMotion() + CalculateNoiseJumping();

        if (_swingTimer > 0)
        {
            HandleSwing(noise);
        }
        else
        {
            HandleIdleOrReset(noise);
        }
    }

    private void HandleSpellCasting()
    {
        ResetSwingState();
        MoveToPosition(CalculateTargetPositionForCasting(), CastingSmoothTime, true);
    }

    private void HandleSwing(Vector3 noise)
    {
        MoveToPosition(GetTargetPosition() + noise, SwingSmoothTime);
        _swingTimer -= Time.deltaTime;
    }

    private void HandleIdleOrReset(Vector3 noise)
    {
        _isSwinging = false;

        if (_isLeftSwing)
        {
            HandleLeftSwingReset();
        }

        MoveToPosition(GetTargetPosition() + noise, _currentSmoothTime);
    }

    private void UpdateOffsetsAndSmoothTime()
    {
        if (WandObject.activeSelf)
        {
            _currentSmoothTime = WandSmoothTime;
            _currentTargetOffset = WandOffset;
            _currentRotationOffset = WandRotationOffset;
        }
        else
        {
            _currentSmoothTime = UnarmedSmoothTime;
            _currentTargetOffset = UnarmedOffset;
            _currentRotationOffset = UnarmedRotationOffset;
        }
    }

    private void ResetSwingState()
    {
        _isSwinging = false;
        _isLeftSwing = false;
        _swingTimer = 0f;
    }

    private void HandleLeftSwingReset()
    {
        _swingTimer -= Time.deltaTime;
        if (_swingTimer <= -0.3f)
        {
            _isLeftSwing = false;
        }
    }

    private void MoveToPosition(Vector3 targetPosition, float smoothTime, bool useDelay = false)
    {
        float effectiveSmoothTime;

        if (useDelay)
        {
            _interpolationDelayTimer += Time.deltaTime;

            float t = Mathf.Clamp01((_interpolationDelayTimer) * TransitionSpeed);
            effectiveSmoothTime = Mathf.Lerp(_currentSmoothTime, smoothTime, t);

        }
        else
        {
            effectiveSmoothTime = smoothTime;
            _interpolationDelayTimer = 0f;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, effectiveSmoothTime);

        UpdateRotation();
    }

    private Vector3 GetTargetPosition()
    {
        return _cameraTransform.TransformPoint(_currentTargetOffset);
    }

    public void ToggleWand()
    {
        if (_isToggleWandLocked) return; // Zablokowane, wyjdź z funkcji

        _isToggleWandLocked = true; // Zablokuj kliknięcia

        if (GameState.Instance.IsWandEquipped)
        {
            Animator.SetBool("isHoldingWand", false);
            GameState.Instance.IsWandEquipped = false;
        }
        else
        {
            Animator.SetBool("isHoldingWand", true);
        }

    }

    public void UnlockToggleWand()
    {
        _isToggleWandLocked = false;
        if (Animator.GetBool("isHoldingWand"))
        {
            GameState.Instance.IsWandEquipped = true;
        }
    }

    public void TogglePrimaryAttack()
    {
        if (GameState.Instance.IsWandEquipped)
        {
            _isSwinging = true;
            _swingTimer = SwingHoldTime;
            _isLeftSwing = !_isLeftSwing;

            if (_isLeftSwing)
            {
                _currentTargetOffset = LeftSwingOffset;
                _currentRotationOffset = LeftSwingRotationOffset;
            }
            else
            {
                _currentTargetOffset = RightSwingOffset;
                _currentRotationOffset = RightSwingRotationOffset;
            }
        }
    }

    private Vector3 CalculateNoiseMotion()
    {
        float speed = new Vector3(CharacterController.velocity.x, 0, CharacterController.velocity.z).magnitude;
        if (speed < SpeedThreshold || !CharacterController.isGrounded) return Vector3.zero;

        if (speed > 5) NoiseFrequency = 18.0f;
        else if (speed > 3) NoiseFrequency = 14.0f;
        else NoiseFrequency = 10.0f;

        // Huśtanie się w lokalnych osiach kamery (lewo-prawo) niezależnie od jej obrotu
        Vector3 localMotion = new Vector3(
            Mathf.Cos(Time.time * NoiseFrequency / 2) * NoiseAmplitude * 2, // Ruch w lewo-prawo
            Mathf.Sin(Time.time * NoiseFrequency) * NoiseAmplitude,          // Ruch w górę-dół
            0                                                                // Brak ruchu do przodu/tyłu
        );

        return _cameraTransform.TransformDirection(localMotion); // Konwersja na przestrzeń świata
    }

    private Vector3 CalculateNoiseJumping()
    {
        float verticalSpeed = CharacterController.velocity.y;
        if (CharacterController.isGrounded) return Vector3.zero;

        float verticalOffset = -Mathf.Sign(verticalSpeed) * Mathf.Abs(verticalSpeed) * JumpNoiseAmplitude;
        verticalOffset *= Mathf.Clamp01(Mathf.Abs(verticalSpeed) / 10.0f); // np. ograniczenie odchylenia
        Vector3 localJumpingMotion = new Vector3(0, verticalOffset, 0);
        return _cameraTransform.TransformDirection(localJumpingMotion);
    }


    private void UpdateRotation()
    {
        // Ustawienie docelowych kątów pitch i yaw na podstawie pozycji kamery
        float targetPitch = _cameraTransform.eulerAngles.x;
        float targetYaw = _cameraTransform.eulerAngles.y;

        // Płynne przejście pitch (góra-dół) i yaw (lewo-prawo)
        _currentPitch = Mathf.LerpAngle(_currentPitch, targetPitch, Time.deltaTime / RotationSmoothTime);
        _currentYaw = Mathf.LerpAngle(_currentYaw, targetYaw, Time.deltaTime / HorizontalRotationSmoothTime);
        _currentYaw = Mathf.Repeat(_currentYaw, 360f);

        // Tworzymy nową docelową rotację z zaktualizowanymi wartościami pitch i yaw
        _targetRotation = Quaternion.Euler(_currentPitch, _currentYaw, _currentRotationOffset.z) * _currentRotationOffset;

        // Płynne przejście do docelowej rotacji
        _currentRotation = Quaternion.Slerp(_currentRotation, _targetRotation, Time.deltaTime / RotationSmoothTime);

        // Aktualizujemy rotację obiektu na wynik interpolacji
        transform.rotation = _currentRotation;
    }

    private Vector3 CalculateTargetPositionForCasting()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 screenCenter = new(Screen.width / 2, Screen.height / 2, 0);
        Vector3 offsetFromCenter = (mousePosition - screenCenter) * FollowStrength;
        Vector3 scaledPosition = screenCenter + offsetFromCenter;
        scaledPosition.z = DistanceFromCamera;
        return _mainCamera.ScreenToWorldPoint(scaledPosition) + _cameraTransform.TransformDirection(CastingOffset);
    }

    private bool IsAnimatorPlaying(string stateName)
    {
        return Animator.GetCurrentAnimatorStateInfo(0).length >
               Animator.GetCurrentAnimatorStateInfo(0).normalizedTime && Animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    void OnDrawGizmos()
    {
        // Rysuj mały punkt, który wskazuje na obiekt
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);

        // Rysuj linię, która pokazuje, gdzie obiekt wskazuje
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
    }
}