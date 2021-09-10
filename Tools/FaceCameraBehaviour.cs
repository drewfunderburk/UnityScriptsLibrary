using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraBehaviour : MonoBehaviour
{
    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        transform.rotation = _camera.transform.rotation;
    }
}