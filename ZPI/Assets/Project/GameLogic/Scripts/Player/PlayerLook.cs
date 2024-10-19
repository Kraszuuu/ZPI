using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Camera")]
    [Tooltip("Main player camera")]
    public Camera cam;
    [Tooltip("Mouse sensitivity setting")]
    public float Sensitivity = 1.0f;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -90.0f;

    private const float _threshold = 0.002f;
    private float _rotationVelocity;
    private float _cameraPitchAngle;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    public void ProcessLook(Vector2 input)
    {
        if (input.sqrMagnitude >= _threshold)
        {
            _rotationVelocity = input.x * Sensitivity;
            _cameraPitchAngle += input.y * Sensitivity;

            _cameraPitchAngle = ClampAngle(_cameraPitchAngle, BottomClamp, TopClamp);

            cam.transform.localRotation = Quaternion.Euler(_cameraPitchAngle, 0.0f, 0.0f);
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
