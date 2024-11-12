using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform TargetPoint; // Dodaj tutaj referencję do punktu
    public float ShakeDuration = 0.2f;
    public float ShakeIntensity = 0.1f;

    private Vector3 _initialPosition;
    private Vector3 _initialTargetPosition; // Dla punktu
    private float _currentShakeDuration = 0f;

    void Start()
    {
        _initialPosition = transform.localPosition;
        if (TargetPoint != null)
        {
            _initialTargetPosition = TargetPoint.localPosition;
        }
    }

    void Update()
    {
        if (_currentShakeDuration > 0)
        {
            Vector3 randomOffset = Random.insideUnitSphere * ShakeIntensity;
            transform.localPosition = _initialPosition + randomOffset;

            if (TargetPoint != null)
            {
                TargetPoint.localPosition = _initialTargetPosition + randomOffset;
            }

            _currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = _initialPosition;
            if (TargetPoint != null)
            {
                TargetPoint.localPosition = _initialTargetPosition;
            }
        }
    }

    public void Shake()
    {
        _currentShakeDuration = ShakeDuration;
        Debug.Log("Shake!");
    }

    private void IgnoreMinimapLayer()
    {
        // Pobierz kamerę z obiektu, do którego podpięty jest ten skrypt
        Camera cam = GetComponent<Camera>();

        if (cam != null)
        {
            // Wyłącz renderowanie dla określonej warstwy
            int layerMask = 1 << LayerMask.NameToLayer("MinimapElement");
            cam.cullingMask &= ~layerMask;
        }
        else
        {
            Debug.LogWarning("Brak komponentu Camera na obiekcie.");
        }
    }
}
