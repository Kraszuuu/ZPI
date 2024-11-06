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

    public bool _isSwinging = false;      // Czy ręka aktualnie macha
    public bool _isLeftSwing = false;      // Czy ręka aktualnie machnela w lewo
    public float _swingTimer = 0f;        // Timer do zatrzymania w miejscu

    private Vector3 _velocity = Vector3.zero;  // Prędkość punktu, wymagana przez SmoothDamp
    private Camera _mainCamera;
    private Transform _cameraTransform;
    private InputManager _inputManager;
    private bool _isWandEquipped = false;    // Czy gracz trzyma różdżkę

    public Animator animator;

    private float _currentSmoothTime;
    private float _currentYaw;              // Bieżąca wartość obrotu w poziomie (Y)
    private float _currentPitch;            // Bieżąca wartość obrotu w pionie (X)
    [SerializeField] private Vector3 _currentTargetOffset;
    [SerializeField] private Quaternion _currentRotationOffset;

    private Quaternion _targetRotation;
    private Quaternion _currentRotation;

    void Start()
    {
        _mainCamera = Camera.main;
        _inputManager = FindObjectOfType<InputManager>();
        _cameraTransform = _mainCamera.transform;

        _currentSmoothTime = UnarmedSmoothTime;
        _currentRotationOffset = UnarmedRotationOffset;
        _currentTargetOffset = UnarmedOffset;
        _currentYaw = _cameraTransform.eulerAngles.y;
        _currentPitch = _cameraTransform.eulerAngles.x;

        // Na początku różdżka jest ukryta
        if (WandObject != null)
            WandObject.SetActive(false);
    }

    void Update()
    {
        // Obsługa przełączania różdżki
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            if (!IsAnimatorPlaying("WandSheath") && !IsAnimatorPlaying("WandWithdrawing"))
            {
                ToggleWand();
            }
            else Debug.Log("Animation is in progress");
        }

        if (!_isSwinging)
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

        // Sprawdzenie, czy nastąpiło wciśnięcie prawego przycisku myszy do rzucenia zaklęcia
        if (Mouse.current.rightButton.wasPressedThisFrame && _isWandEquipped)
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

        Vector3 noiseMotion = CalculateNoiseMotion();
        Vector3 noiseJumping = CalculateNoiseJumping();

        if (_inputManager.isCastSpelling)
        {
            _isSwinging = false;
            _isLeftSwing = false;
            _swingTimer = 0f;

            Vector3 targetPosition = CalculateTargetPositionForCasting();
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, CastingSmoothTime);
        }
        else if (_swingTimer <= 0)
        {
            _isSwinging = false;
            _isLeftSwing = false;

            Vector3 targetPosition = _cameraTransform.TransformPoint(_currentTargetOffset);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + noiseMotion + noiseJumping, ref _velocity, _currentSmoothTime);
            UpdateRotation();
        }
        else
        {
            Vector3 targetPosition = _cameraTransform.TransformPoint(_currentTargetOffset);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + noiseMotion + noiseJumping, ref _velocity, SwingSmoothTime);
            UpdateRotation();
            _swingTimer -= Time.deltaTime;
        }
    }

    void ToggleWand()
    {
        _isWandEquipped = !_isWandEquipped;

        if (_isWandEquipped)
        {
            animator.SetBool("isHoldingWand", true);
        }
        else
        {
            animator.SetBool("isHoldingWand", false);
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

        // Obliczamy różnicę kątową między bieżącą a docelową rotacją
        float angleDifference = Quaternion.Angle(_currentRotation, _targetRotation);

        // Dynamiczne skalowanie RotationSmoothTime, odwrotnie proporcjonalne do różnicy kątowej
        float dynamicSmoothTime = Mathf.Lerp(RotationSmoothTime * 0.1f, RotationSmoothTime, 1 - (angleDifference / 180f));

        // Płynne przejście do docelowej rotacji z dynamicznym RotationSmoothTime
        _currentRotation = Quaternion.Slerp(_currentRotation, _targetRotation, Time.deltaTime / dynamicSmoothTime);

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

    bool IsAnimatorPlaying(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
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