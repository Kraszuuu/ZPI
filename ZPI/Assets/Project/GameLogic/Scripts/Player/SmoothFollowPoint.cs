using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowPoint : MonoBehaviour
{
    public Transform cameraTransform;      // Referencja do kamery
    public float smoothTime = 0.3f;        // Czas amortyzacji pozycji
    public float rotationSmoothTime = 0.3f; // Czas amortyzacji rotacji
    public float horizontalRotationSmoothTime = 0.3f; // Czas amortyzacji poziomego obrotu (lewo-prawo)

    private Vector3 _localOffset;          // Offset w lokalnych współrzędnych względem kamery
    private Vector3 _velocity = Vector3.zero;  // Prędkość punktu, wymagana przez SmoothDamp
    private Quaternion _rotationOffset;    // Offset rotacji między punktem a kamerą
    private float _currentYaw;             // Bieżąca wartość obrotu w poziomie (Y)
    void Start()
    {
        // Zapisz lokalny offset punktu względem kamery
        _localOffset = cameraTransform.InverseTransformPoint(transform.position);

        // Zapisz offset rotacji (różnicę między kamerą a punktem)
        _rotationOffset = Quaternion.Inverse(cameraTransform.rotation) * transform.rotation;

        // Zainicjalizuj bieżący kąt obrotu w poziomie (Y)
        _currentYaw = cameraTransform.eulerAngles.y;
    }

    void Update()
    {
        // Wyznacz nową pozycję punktu w lokalnych współrzędnych względem obrotu kamery
        Vector3 targetPosition = cameraTransform.TransformPoint(_localOffset);

        // Płynnie przejdź do nowej pozycji z amortyzacją
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);

        // Uwzględnij offset rotacji dla osi pionowej (góra-dół) kamery
        Quaternion targetRotation = cameraTransform.rotation * _rotationOffset;

        // Płynnie przejdź do nowej rotacji w pionie z amortyzacją
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / rotationSmoothTime);

        // Płynne opóźnienie dla obrotu w poziomie (Y)
        float targetYaw = cameraTransform.eulerAngles.y; // Docelowy kąt poziomy kamery
        _currentYaw = Mathf.LerpAngle(_currentYaw, targetYaw, Time.deltaTime / horizontalRotationSmoothTime);

        // Zastosuj płynny obrót tylko w osi Y (poziomej)
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, _currentYaw, transform.eulerAngles.z);
    }
}
