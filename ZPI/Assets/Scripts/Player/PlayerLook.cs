using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;

    public float sensitivity = 30f;

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x * sensitivity * Time.deltaTime;
        float mouseY = input.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, -0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}