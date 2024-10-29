using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollowPoint : MonoBehaviour
{
    [SerializeField] public float smoothTime = 0.2f; // Czas amortyzacji ruchu punktu
    public float distanceFromCamera = 5.0f; // Odległość punktu od kamery

    private Vector3 _velocity = Vector3.zero; // Prędkość punktu używana przez SmoothDamp
    private Camera _mainCamera;              // Referencja do głównej kamery

    void Start()
    {
        // Pobierz główną kamerę
        _mainCamera = Camera.main;
    }

    void Update()
    {
        // Zdobądź pozycję myszy na ekranie
        Vector3 mousePosition = Input.mousePosition;
        // Ustaw odległość punktu od kamery w przestrzeni 3D
        mousePosition.z = distanceFromCamera;

        // Przekształć pozycję myszy na współrzędne światowe
        Vector3 targetPosition = _mainCamera.ScreenToWorldPoint(mousePosition);

        // Płynnie przesuń punkt w kierunku docelowej pozycji
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
    }

    void OnDrawGizmos()
    {
        // Rysuj mały punkt w pozycji obiektu, aby łatwo zobaczyć efekt w trybie Scene
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
