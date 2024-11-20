using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
    }
    private void Update()
    {
        transform.forward = _cam.transform.forward;
    }
}
