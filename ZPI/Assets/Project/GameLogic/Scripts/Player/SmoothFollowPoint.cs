using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowPoint : MonoBehaviour
{
    public Transform cameraTransform;       // Referencja do kamery
    public float smoothTime = 0.3f;         // Czas amortyzacji pozycji
    public float rotationSmoothTime = 0.3f; // Czas amortyzacji rotacji
    public float horizontalRotationSmoothTime = 0.3f; // Czas amortyzacji poziomego obrotu (lewo-prawo)

    private Vector3 _localOffset;           // Offset w lokalnych współrzędnych względem kamery
    private Vector3 _velocity = Vector3.zero;  // Prędkość punktu, wymagana przez SmoothDamp
    private Quaternion _rotationOffset;     // Offset rotacji między punktem a kamerą
    private float _currentYaw;              // Bieżąca wartość obrotu w poziomie (Y)
    private float _currentPitch;            // Bieżąca wartość obrotu w pionie (X)

    void Start()
    {
        // Zapisz lokalny offset punktu względem kamery
        _localOffset = cameraTransform.InverseTransformPoint(transform.position);

        // Zapisz offset rotacji (różnicę między kamerą a punktem)
        _rotationOffset = Quaternion.Inverse(cameraTransform.rotation) * transform.rotation;

        // Zainicjalizuj bieżący kąt obrotu w poziomie (Y)
        _currentYaw = cameraTransform.eulerAngles.y;
        // Zainicjalizuj bieżący kąt obrotu w pionie (X)
        _currentPitch = cameraTransform.eulerAngles.x;
    }

    void Update()
    {
        // Wyznacz nową pozycję punktu w lokalnych współrzędnych względem obrotu kamery
        Vector3 targetPosition = cameraTransform.TransformPoint(_localOffset);

        // Płynnie przejdź do nowej pozycji z amortyzacją
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);

        // Płynne opóźnienie dla obrotu w pionie (X)
        float targetPitch = cameraTransform.eulerAngles.x; // Docelowy kąt pionowy kamery
        _currentPitch = Mathf.LerpAngle(_currentPitch, targetPitch, Time.deltaTime / rotationSmoothTime);

        // Płynne opóźnienie dla obrotu w poziomie (Y)
        float targetYaw = cameraTransform.eulerAngles.y; // Docelowy kąt poziomy kamery
        _currentYaw = Mathf.LerpAngle(_currentYaw, targetYaw, Time.deltaTime / horizontalRotationSmoothTime);

        // Utwórz nową rotację uwzględniającą interpolowane kąty oraz offset rotacji
        Quaternion interpolatedRotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        transform.rotation = interpolatedRotation * _rotationOffset;  // Zastosuj offset rotacji
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
